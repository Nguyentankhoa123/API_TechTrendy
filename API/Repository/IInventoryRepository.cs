using API.Model.Dtos.InventoryDto;

namespace API.Repository
{
    public interface IInventoryRepository
    {
        Task<InventoryObjectResponse> InsertInventory(int productId, int stock, string location);

        Task<int> ReservationInventory(int productId, int quantity, int cartId);
    }
}
