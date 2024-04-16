using API.Data;
using API.Model.Entity;
using API.Repositories;
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


        public async Task<dynamic> GetAllProducts(int pageNumber = 1, int pageSize = 5)
        {
            var skipResults = (pageNumber - 1) * pageSize;
            var laptop = await _storeContext.Products.OfType<Laptop>()
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .ToListAsync();

            var tablet = await _storeContext.Products.OfType<Tablet>()
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Inventory)
                .ToListAsync();

            return new
            {
                laptop,
                tablet
            };
        }


        public async Task<Product?> GetProductById(int id)
        {
            return await _storeContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        }


    }
}
