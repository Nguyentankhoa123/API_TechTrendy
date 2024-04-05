using API.Model.Dtos.LaptopDto;
using API.Model.Entity;

namespace API.Repository
{
    public interface ILaptopRepository
    {
        Task<LaptopListObjectResponse> GetAsync(int pageNumber, int pageSize);

        Task<LaptopObjectResponse> GetIdAsync(int id);

        Task<LaptopListObjectResponse> SearchAsync(string? brandQuery, string? priceSortOrder, int pageNumber, int pageSize);

        Task<LaptopObjectResponse> CreateAsync(int categoryId, int brandId, Laptop laptop);

        Task<LaptopObjectResponse> DeleteAsync(int id);

        Task<LaptopObjectResponse> UpdateAsync(int id, LaptopRequest laptop);
    }
}
