using API.Model.Dtos.TabletDto;
using API.Model.Entity;

namespace API.Repository
{
    public interface ITabletRepository
    {
        Task<TabletListObjectResponse> GetAsync(int pageNumber, int pageSize);

        Task<TabletObjectResponse> GetIdAsync(int id);
        Task<TabletObjectResponse> CreateAsync(int categoryId, int brandId, Tablet tablet);

        Task<TabletObjectResponse> DeleteAsync(int id);

        Task<TabletObjectResponse> UpdateAsync(int id, TabletRequest tablet);

        Task<TabletListObjectResponse> SearchAsync(string? brandQuery, string? priceSortOrder, int pageNumber, int pageSize);
    }
}
