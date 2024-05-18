using API.Data;
using API.Model.Dtos.DiscountDto;
using API.Model.Dtos.ProductDto;
using API.Model.Entity;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace API.Repository.Implement
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public DiscountRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }

        public async Task<DiscountObjectResponse> CreateDiscountCode(DiscountRequest request, List<int>? productId)
        {

            // Check if the start date and end date are valid
            if (DateTime.Now > request.StartDate || DateTime.Now > request.EndDate)
            {
                throw new BadRequestException("Discount code has expried!");
            }

            if (request.StartDate >= request.EndDate)
            {
                throw new BadRequestException("Start date must be before End Date");
            }

            var foundDiscount = await _storeContext.Discounts.FirstOrDefaultAsync(x => x.Code == request.Code);

            if (foundDiscount != null && foundDiscount.IsActive)
            {
                throw new BadRequestException("Discount exists!");
            }

            var discount = _mapper.Map<Discount>(request);

            if (productId != null && productId.Any())
            {
                var products = await _storeContext.Products.Where(p => productId.Contains(p.Id)).ToListAsync();
                if (products == null)
                {
                    throw new NotFoundException("Product not found");
                }

                discount.ApplyTo = DiscountApply.Specific;
                discount.Products = products;
                discount.ProductId = productId;
            }
            else
            {
                discount.ApplyTo = DiscountApply.All;

            }

            _storeContext.Discounts.Add(discount);
            await _storeContext.SaveChangesAsync();

      

            var response = new DiscountObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Discount created",
                Data = _mapper.Map<DiscountResponse>(discount)
            };

            response.Data.ProductId = productId;


            return response;
        }

        public async Task<DiscountProductObjectResponse> GetAllDiscountWithProduct(string code, int pageNumber, int pageSize)
        {
            var foundDiscount = await _storeContext.Discounts.FirstOrDefaultAsync(x => x.Code == code);

            if (foundDiscount == null || foundDiscount.IsActive == false)
            {
                throw new NotFoundException("Discount not exists!");
            }

            var skipResults = (pageNumber - 1) * pageSize;

            var response = new DiscountProductObjectResponse
            {
                StatusCode = ResponseCode.OK
            };
            if (foundDiscount.ApplyTo == DiscountApply.All)
            {
                var products = await _storeContext.Products
                    .Skip(skipResults)
                    .Take(pageSize)
                    .Select(p => new ProductDiscountReponse { Id = p.Id, Name = p.Name })
                    .ToListAsync();

                response.Message = "Get all products";
                response.Data = _mapper.Map<DiscountProductResponse>(foundDiscount);
                response.Data.Products = products;

            }
            else if (foundDiscount.ApplyTo == DiscountApply.Specific)
            {

                var products = await _storeContext.Products
                    .Where(p => p.DiscountId == foundDiscount.Id)
                    .Skip(skipResults)
                    .Take(pageSize)
                    .Select(p => new ProductDiscountReponse { Id = p.Id, Name = p.Name })
                    .ToListAsync();

                response.Message = "Get specific products";
                response.Data = _mapper.Map<DiscountProductResponse>(foundDiscount);
                response.Data.Products = products;
            }
            return response;
        }

        public async Task<DiscountUserObjectResponse> GetAllDiscountsForUser(string userId)
        {
            var userDiscount = await _storeContext.DiscountsUser
                .Where(du => du.UserId == userId && !du.IsActive)
                .Select(du => du.DiscountId)
                .ToListAsync();

            var availableDiscounts = await _storeContext.Discounts
                .Where(d => d.IsActive && d.UseCount < d.MaxUse && !userDiscount.Contains(d.Id))
                .ToListAsync();

            var response = new DiscountUserObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get all active codes",
                Data = availableDiscounts.Select(d => _mapper.Map<DiscountUserResponse>(d)).ToList()
            };

            return response;
        }

        public async Task<DiscountObjectResponse> DeleteDiscountCode(int codeId)
        {
            var discount = await _storeContext.Discounts.FirstOrDefaultAsync(x => x.Id == codeId);

            if (discount == null)
            {
                throw new NotFoundException("Discount code not found.");
            }

            _storeContext.Discounts.Remove(discount);
            await _storeContext.SaveChangesAsync();

            var response = new DiscountObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Discount code deleted successfully",
            };

            return response;

        }
    }
}
