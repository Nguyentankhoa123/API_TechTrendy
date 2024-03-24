using API.Model.Dtos.CartDto;

namespace API.Repositories
{
    public interface ICartRepository
    {
        Task<CartObjectResponse> GetCartAsync(string userId);
        Task<CartObjectResponse> AddAsync(string userId, int productId, int quantity);

        Task<CartObjectResponse> DecreaseAsync(string userId, int productId, int quantity);

        Task<CartObjectResponse> RemoveAsync(string userId, int productId);

        Task<CartObjectResponse> RemoveAllAsync(string userId);
    }
}
