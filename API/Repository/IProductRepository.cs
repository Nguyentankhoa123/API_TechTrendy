using API.Model.Entity;

namespace API.Repositories
{
    public interface IProductRepository
    {
        Task<dynamic> GetAllProducts(int pageNumber, int pageSize);

        Task<Product> GetProductById(int id);

        Task<dynamic> SearchAsync(string nameQuery, int pageNumber, int pageSize);

        Task<Product> DeleteProduct(int id);
    }
}
