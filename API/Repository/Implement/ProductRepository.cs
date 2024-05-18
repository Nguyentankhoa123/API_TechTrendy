using API.Data;
using API.Model.Entity;
using API.Repositories;
using API.Services.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public ProductRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }

        public async Task<Product> DeleteProduct(int id)
        {
            var product = await _storeContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                throw new NotFoundException("Product not found");
            }

            _storeContext.Products.Remove(product);
            await _storeContext.SaveChangesAsync();

            return product;
        }

        public async Task<dynamic> GetAllProducts(int pageNumber = 1, int pageSize = 5)
        {
            var skipResults = (pageNumber - 1) * pageSize;
            var laptopProducts = await _storeContext.Products.OfType<Laptop>()
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .ToListAsync();

            var tabletProducts = await _storeContext.Products.OfType<Tablet>()
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .ToListAsync();

            return new
            {
                Laptops = laptopProducts,
                Tablets = tabletProducts
            };
        }


        public async Task<Product?> GetProductById(int id)
        {
            return await _storeContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<dynamic> SearchAsync(string nameQuery, int pageNumber, int pageSize)
        {
            var products = _storeContext.Products.AsQueryable();

            // filtering
            if (!string.IsNullOrWhiteSpace(nameQuery))
            {
                products = products.Where(x => x.Name != null && x.Name.Contains(nameQuery));
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            var result = await products
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .ToListAsync();

            if (result == null || result.Count == 0)
            {
                throw new NotFoundException("No products found");
            }

            var laptopProducts = result.OfType<Laptop>().ToList();
            var tabletProducts = result.OfType<Tablet>().ToList();

            return new
            {
                Laptops = laptopProducts,
                Tablets = tabletProducts
            };
        }

    }
}
