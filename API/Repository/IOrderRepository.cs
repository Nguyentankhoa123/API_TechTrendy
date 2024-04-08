using API.Model.Dtos.OrderDto;

namespace API.Repositories
{
    public interface IOrderRepository
    {
        Task<List<OrderObjectResponse>> GetOrderAsync(string userId);
        Task<OrderObjectResponse> CreateOrderAsync(OrderRequest orderDto, string userId, string? code, int addressId, HttpContext context);
    }
}
