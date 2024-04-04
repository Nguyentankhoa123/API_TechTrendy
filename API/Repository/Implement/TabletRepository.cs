using API.Data;
using API.Model.Dtos.TabletDto;
using API.Model.Entity;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class TabletRepository : ITabletRepository
    {
        private readonly IMapper _mapper;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly StoreContext _storeContext;
        public TabletRepository(StoreContext storeContext, IMapper mapper, IInventoryRepository inventoryRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
        }
        public async Task<TabletObjectResponse> CreateAsync(int categoryId, int brandId, Tablet tablet)
        {
            var category = await _storeContext.Categories.FindAsync(categoryId);
            var brand = await _storeContext.Brands.FindAsync(brandId);

            if (category == null || brand == null)
            {
                throw new NotFoundException("Category or Brand missing");
            }

            tablet.Category = category;
            tablet.Brand = brand;

            _storeContext.Tablets.Add(tablet);
            await _storeContext.SaveChangesAsync();

            var inventoryResponse = await _inventoryRepository.InsertInventory(tablet.Id, tablet.Quantity, "Default Location");


            var tabletResponse = _mapper.Map<TabletResponse>(tablet);

            var response = new TabletObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Tablet created successfully",
                Data = tabletResponse
            };

            return response;
        }

        public async Task<TabletObjectResponse> DeleteAsync(int id)
        {
            var tablet = await _storeContext.Tablets
                .Include(t => t.Brand)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (tablet == null)
            {
                throw new NotFoundException("Tablet not found");
            }

            _storeContext.Tablets.Remove(tablet);
            await _storeContext.SaveChangesAsync();

            var response = new TabletObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Tablet deleted successfully",
                Data = _mapper.Map<TabletResponse>(tablet)
            };

            return response;
        }

        public async Task<TabletListObjectResponse> GetAsync(int pageNumber, int pageSize)
        {
            var tablets = await _storeContext.Products.OfType<Tablet>()
                .Include(t => t.Brand)
                .Include(t => t.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var tabletResponses = _mapper.Map<List<TabletResponse>>(tablets);

            if (!tablets.Any())
            {
                throw new NotFoundException("No tablets found.");
            }

            var response = new TabletListObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get all tablet",
                Data = tabletResponses
            };

            return response;
        }

        public async Task<TabletObjectResponse> GetIdAsync(int id)
        {
            var tablet = await _storeContext.Tablets
                .Include(t => t.Category)
                .Include(t => t.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tablet == null)
            {
                throw new NotFoundException("Tablet not found");
            }

            var response = new TabletObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get tablet id successful",
                Data = _mapper.Map<TabletResponse>(tablet)
            };

            return response;
        }

        public async Task<TabletListObjectResponse> SearchAsync(string? brandQuery, string? priceSortOrder, int pageNumber, int pageSize)
        {
            var products = _storeContext.Products.OfType<Tablet>().AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(brandQuery))
            {
                products = products.Where(x => x.Brand != null && x.Brand.Name.Contains(brandQuery));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(priceSortOrder))
            {
                if (priceSortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.OrderByDescending(x => x.Price);
                }
                else if (priceSortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.OrderBy(x => x.Price);
                }
            }
            else
            {
                // Price descending
                products = products.OrderByDescending(x => x.Price);
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            var result = await products
                .Skip(skipResults)
                .Take(pageSize)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

            if (result == null || result.Count == 0)
            {
                throw new NotFoundException("No tablets found");
            }

            var response = new TabletListObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get tablets successful",
                Data = _mapper.Map<List<TabletResponse>>(result)
            };

            return response;
        }

        public async Task<TabletObjectResponse> UpdateAsync(int id, TabletRequest tablet)
        {
            var existingTablet = await _storeContext.Tablets
                .Include(t => t.Brand)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingTablet == null)
            {
                throw new NotFoundException("Tablet not found");
            }

            _mapper.Map(tablet, existingTablet);


            await _storeContext.SaveChangesAsync();

            var updatedTablet = _mapper.Map<TabletResponse>(existingTablet); // Map sang TabletResponse

            var response = new TabletObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Tablet updated successfully",
                Data = updatedTablet
            };

            return response;
        }
    }
}
