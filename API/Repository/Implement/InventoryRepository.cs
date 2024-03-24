using API.Data;
using API.Model.Dtos.InventoryDto;
using API.Model.Entity;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public InventoryRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }

        public async Task<InventoryObjectResponse> InsertInventory(int productId, int stock, string location)
        {
            var inventory = new Inventory
            {
                ProductId = productId,
                Stock = stock,
                Location = location
            };

            // Thêm đối tượng Inventory vào DbContext
            _storeContext.Inventories.Add(inventory);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _storeContext.SaveChangesAsync();

            var response = new InventoryObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Create inventory",
                Data = _mapper.Map<InventoryResponse>(inventory)
            };

            return response;
        }

        public async Task<int> ReservationInventory(int productId, int quantity, int cartId)
        {
            var inventory = await _storeContext.Inventories
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.Stock >= quantity);


            if (inventory == null)
            {
                return 0;
            }

            // Decrease the stock
            inventory.Stock -= quantity;

            var reservation = new Reservation
            {
                Quantity = quantity,
                CartId = cartId,
                CreateOn = DateTime.Now
            };

            inventory.Reservations.Add(reservation);

            return await _storeContext.SaveChangesAsync();

        }
    }
}
