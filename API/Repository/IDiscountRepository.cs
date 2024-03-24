using API.Model.Dtos.DiscountDto;

namespace API.Repository
{
    public interface IDiscountRepository
    {
        Task<DiscountObjectResponse> CreateDiscountCode(DiscountRequest request, List<int>? productId);

        Task<DiscountProductObjectResponse> GetAllDiscountWithProduct(string code, int pageNumber, int pageSize);

        Task<DiscountUserObjectResponse> GetAllDiscountsForUser(string userId);

        Task<DiscountObjectResponse> DeleteDiscountCode(int codeId);
    }
}
