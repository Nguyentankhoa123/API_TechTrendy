using API.Data;
using API.Helpers;
using API.Model.Dtos.OrderDto;
using API.Model.Dtos.VnPayDto;
using API.Model.Entity;
using API.Repositories;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Scriban;
using System.Globalization;

namespace API.Repository.Implement
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly IEmailSenderRepository _emailSender;
        private readonly IRedisRepository _redisRepository;
        private readonly IConfiguration _config;

        public OrderRepository(StoreContext storeContext, IMapper mapper, IEmailSenderRepository emailSender, IRedisRepository redisRepository, IConfiguration config)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _emailSender = emailSender;
            _redisRepository = redisRepository;
            _config = config;
        }

        public async Task<OrderObjectResponse> CreateOrderAsync(OrderRequest request, string userId, string? code, int addressId, HttpContext context)
        {

            // Fetch the user's information from the database
            var user = await _storeContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            var address = await _storeContext.UserAddresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);
            // If the user does not exist, throw an exception
            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            // Fetch the user's cart from the database
            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // If the cart does not exist or is empty, throw an exception
            if (cart == null || !cart.CartItems.Any())
            {
                throw new NotFoundException("The cart does not exist or is empty.");
            }

            var order = _mapper.Map<Order>(request);
            // Liên kết Order với User
            order.User = user;


            order.OrderDetails = cart.CartItems.Select(ci => new OrderDetail
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                Total = ci.Quantity * ci.Product.Price,
                Name = ci.Product.Name,
                PictureUrls = ci.Product.PictureUrls.FirstOrDefault()
            }).ToList();

            foreach (var detail in order.OrderDetails)
            {
                string lockKey = await _redisRepository.AcquireLockAsync(detail.ProductId, detail.Quantity, cart.Id);
                if (lockKey == null)
                {
                    throw new Exception($"{userId} Failed to acquire lock for product {detail.ProductId} vì đã hết hàng trong kho");
                }
            }

            double totalDiscountAmount = 0;
            if (!string.IsNullOrEmpty(code))
            {
                var discount = await _storeContext.Discounts.FirstOrDefaultAsync(d => d.Code == code);

                var discountUser = await _storeContext.DiscountsUser.FirstOrDefaultAsync(du => du.DiscountId == discount.Id && du.UserId == userId);


                if (discount == null)
                {
                    throw new NotFoundException("Discount doesn't exitst");
                }

                if (discountUser != null && !discountUser.IsActive)
                {
                    throw new NotFoundException("Discount expried for this user");
                }

                if (discount.MaxUse == 0 || discount.UseCount >= discount.MaxUse)
                {
                    throw new NotFoundException("Discount are out");
                }

                if (DateTime.Now < discount.StartDate || DateTime.Now > discount.EndDate)
                {
                    throw new NotFoundException("Discount code has expired");
                }

                if (discount != null && discount.IsActive)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        if (discount.ApplyTo == DiscountApply.All || (discount.ApplyTo == DiscountApply.Specific && discount.ProductId.Contains(detail.ProductId)))
                        {

                            var discountAmount = detail.Total * discount.Percent / 100;
                            discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount.Value);

                            totalDiscountAmount += discountAmount;
                        }
                    }

                    discount.UseCount += 1;
                    if (discount.UseCount >= discount.MaxUse)
                    {
                        discount.IsActive = false;
                    }
                    await _storeContext.SaveChangesAsync();

                    if (discountUser == null)
                    {
                        discountUser = new DiscountUser
                        {
                            DiscountId = discount.Id,
                            UserId = userId,
                            IsActive = false,
                            UsedAt = DateTime.Now
                        };
                        _storeContext.DiscountsUser.Add(discountUser);
                    }
                    else
                    {
                        discountUser.IsActive = false;
                        discountUser.UsedAt = DateTime.Now;
                    }

                    await _storeContext.SaveChangesAsync();
                }
            }


            order.DiscountAmount = totalDiscountAmount;
            order.TotalPrice = order.OrderDetails.Sum(od => od.Total) - totalDiscountAmount;

            if (user == null || order == null)
            {
                throw new Exception("User or Order is null.");
            }

            string paymentUrl = null;
            if (request.PaymentType == "VnPay")
            {
                var vnPayRequest = new VnPayRequest
                {
                    OrderId = order.Id,
                    Amount = order.TotalPrice,
                    CreatedDate = DateTime.Now
                };

                var vnPayRepo = new VnPayRepository(_config);
                paymentUrl = vnPayRepo.CreatePaymentUrl(context, vnPayRequest);

            }
            else if (request.PaymentType == "cod")
            {
            }


            _storeContext.Orders.Add(order);
            _storeContext.Carts.Remove(cart);
            await _storeContext.SaveChangesAsync();

            // Convert the UTC DateTime to local time
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(order.CreatedAt, vietnamTimeZone);

            var totalPrice = order.TotalPrice.ToString("C", new CultureInfo("vi-VN"));
            var discountPrice = totalDiscountAmount.ToString("C", new CultureInfo("vi-VN"));
            var price = "";
            foreach (var item in order.OrderDetails)
            {
                price = item.Product.Price.ToString("C", new CultureInfo("vi-VN"));
            };
            var template = Template.Parse(@"
                <!DOCTYPE html>
                <html lang=""en"">
                  <head>
                    <meta charset=""UTF-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                  </head>
                  <body>
                    <div style=""line-height: 18pt"">
                      <p>Xin chào {{user.first_name}} {{user.last_name}}</p>
                      <p>
                        Sản phẩm trong đơn hàng của Anh/chị tại cửa hàng
                        <strong>Tech Trendy</strong>
                      </p>
                      <div style=""font-size: 18px"">Thông tin giao hàng</div>
                      <table>
                        <tbody>
                          <tr>
                            <td>Mã đơn hàng: <strong>{{order.tracking_number}}</strong></td>
                            <td style=""padding-left: 40px"">Ngày tạo giao hàng: {{time | date.to_string '%d/%m/%Y %I:%M:%S %p'}}</td>
                          </tr>
                        </tbody>
                      </table>
                      <table style=""width: 100%; border-collapse: collapse"">
                        <thead>
                          <tr>
                            <th
                              style=""
                                border-left: 1px solid #d7d7d7;
                                border-right: 1px solid #d7d7d7;
                                border-top: 1px solid #d7d7d7;
                                padding: 5px 10px;
                                text-align: left;
                              ""
                            >
                              Thông tin người nhận
                            </th>
                            <th
                              style=""
                                border-left: 1px solid #d7d7d7;
                                border-right: 1px solid #d7d7d7;
                                border-top: 1px solid #d7d7d7;
                                padding: 5px 10px;
                                text-align: left;
                              ""
                            >
                              Thông tin vận chuyển
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr>
                            <td
                              style=""
                                border-left: 1px solid #d7d7d7;
                                border-right: 1px solid #d7d7d7;
                                border-bottom: 1px solid #d7d7d7;
                                padding: 5px 10px;
                              ""
                            >
                              <table style=""width: 100%"">
                                <tbody>
                                  <tr>
                                    <td>Họ tên:</td>
                                    <td>{{address.name}}</td>
                                  </tr>
                                  <tr>
                                    <td>Điện thoại:</td>
                                    <td>{{address.contact}}</td>
                                  </tr>
                                  <tr>
                                    <td>Địa chỉ:</td>
                                    <td>{{address.street}}</td>
                                  </tr>
                                  <tr>
                                    <td>Phường xã:</td>
                                    <td>{{address.ward}}</td>
                                  </tr>
                                  <tr>
                                    <td>Quận huyện:</td>
                                    <td>{{address.district}}</td>
                                  </tr>
                                  <tr>
                                    <td>Tỉnh thành:</td>
                                    <td>{{address.province}}</td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                            <td
                              style=""
                                border-left: 1px solid #d7d7d7;
                                border-right: 1px solid #d7d7d7;
                                border-bottom: 1px solid #d7d7d7;
                                padding: 5px 10px;
                              ""
                            >
                              <table style=""width: 100%"">
                                <tbody>
                                  <tr>
                                    <td><strong>Mã giao vận:</strong></td>
                                    <td>{{order.tracking_number}}</td>
                                  </tr>
                                  <tr>
                                    <td><strong>Đơn vị: </strong></td>
                                    <td>{{order.carrier}}</td>
                                  </tr>
                                  <tr>
                                    <td><strong>Phương thức:</strong></td>
                                    <td>{{order.payment_type}}</td>
                                  </tr>
<tr>
                                    <td><strong>Tổng giá tiền Voucher giảm giá:</strong></td>
                                    <td>{{discount_price}}</td>
                                  </tr>
                                </tbody>
                              </table>
                              <br />
                              Hãy mua đơn từ 750.000 để được Free Ship nhé
                            </td>
                          </tr>
                        </tbody>
                      </table>
                      <table style=""width: 100%; border-collapse: collapse"">
                        <thead>
                          <tr
                            style=""
                              border: 1px solid #d7d7d7;
                              border-top: none;
                              background-color: #f8f8f8;
                            ""
                          >
                            <th style=""text-align: left; padding: 5px 10px"">
                              <strong>Sản phẩm được giao</strong>
                            </th>
                            <th style=""text-align: left""><strong>Giá tiền</strong></th>   
                            <th style=""text-align: center; padding: 5px 10px"">
                              <strong>Số lượng</strong>
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                        {{- for item in order.order_details }}
                          <tr style=""border: 1px solid #d7d7d7"">
                            <td style=""padding: 5px 10px"">
                                 {{item.product.name}}
                            </td>
                            <td>{{price}}</td>
                            <td style=""text-align: center; padding: 5px 10px"">{{item.quantity}}</td>
                          </tr>
                        {{- end }}     
                          <tr style=""border: 1px solid #d7d7d7"">
                            <td colspan=""2""></td>
                            <td colspan=""2"">
                              <table style=""width: 100%"">
                                <tbody>
                                  <tr>
                                    <td>
                                      <strong>Giá trị đơn hàng</strong>
                                    </td>
                                    <td style=""text-align: right"">{{total_price}}</td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </body>
                </html>");

            var response = new OrderObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Order successfully",
                Data = _mapper.Map<OrderResponse>(order)
            };
            response.Data.PaymentUrl = paymentUrl;

            if (response.StatusCode == ResponseCode.OK)
            {
                var emailContent = await template.RenderAsync(new { user = user, order = order, address = address, total_price = totalPrice, price = price, time = localCreatedAt, discount_price = discountPrice });
                var message = new Message(new[] { user.Email }, "Order Confirmation", emailContent);
                _emailSender.SendEmail(message);
            }

            return response;
        }

        public async Task<List<OrderObjectResponse>> GetOrderAsync(string userId)
        {
            // Fetch the user's orders from the database
            var orders = await _storeContext.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                    .ThenInclude(u => u.Address)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            // If the user has no orders, throw an exception
            if (!orders.Any())
            {
                throw new BadRequestException("Không có order nào");
            }

            // Map each order to an OrderObjectResponse
            var response = orders.Select(order => new OrderObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Order fetched successfully",
                Data = _mapper.Map<OrderResponse>(order)
            }).ToList();

            return response;
        }
    }
}
