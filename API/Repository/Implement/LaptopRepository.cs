using API.Data;
using API.Model.Dtos.LaptopDto;
using API.Model.Entity;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class LaptopRepository : ILaptopRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public LaptopRepository(StoreContext storeContext, IMapper mapper, IInventoryRepository inventoryRepository, ICloudinaryRepository cloudinaryRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _cloudinaryRepository = cloudinaryRepository;
        }

        public async Task<LaptopObjectResponse> CreateAsync(int categoryId, int brandId, Laptop laptop)
        {
            // Find the Category and Brand from the database
            var category = await _storeContext.Categories.FindAsync(categoryId);
            var brand = await _storeContext.Brands.FindAsync(brandId);

            if (category == null || brand == null)
            {
                throw new NotFoundException("Category or Brand missing");
            }

            // Assign the Category and Brand to the Laptop
            laptop.Category = category;
            laptop.Brand = brand;


            _storeContext.Laptops.Add(laptop);
            await _storeContext.SaveChangesAsync();

            var inventoryResponse = await _inventoryRepository.InsertInventory(laptop.Id, laptop.Quantity, "Default Location");

            var laptopResponse = _mapper.Map<LaptopResponse>(laptop);

            var response = new LaptopObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Laptop created successfully",
                Data = laptopResponse
            };

            return response;
        }

        public async Task<LaptopObjectResponse> DeleteAsync(int id)
        {
            var laptop = await _storeContext.Laptops.FindAsync(id);
            if (laptop == null)
            {
                throw new NotFoundException("Laptop not found");
            }

            _storeContext.Laptops.Remove(laptop);
            await _storeContext.SaveChangesAsync();

            var response = new LaptopObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Laptop deleted successfully",
                Data = _mapper.Map<LaptopResponse>(laptop)
            };

            return response;
        }

        public async Task<LaptopListObjectResponse> GetAsync(int pageNumber, int pageSize)
        {
            var laptops = await _storeContext.Products.OfType<Laptop>()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var laptopResponses = _mapper.Map<List<LaptopResponse>>(laptops);

            if (!laptops.Any())
            {
                throw new NotFoundException("No laptops found.");
            }

            var response = new LaptopListObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get all laptop",
                Data = laptopResponses
            };

            return response;
        }


        public async Task<LaptopObjectResponse> GetIdAsync(int id)
        {
            var laptop = await _storeContext.Laptops
                .Include(l => l.Category)
                .Include(l => l.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (laptop == null)
            {
                throw new NotFoundException("Laptop not found");
            }

            var response = new LaptopObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get laptop id successful",
                Data = _mapper.Map<LaptopResponse>(laptop)
            };

            return response;
        }

        public async Task<LaptopListObjectResponse> SearchAsync(string? brandQuery, string? priceSortOrder, int pageNumber, int pageSize)
        {
            var products = _storeContext.Products.OfType<Laptop>().AsQueryable();

            // filtering
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
                throw new NotFoundException("No laptops found");
            }

            var response = new LaptopListObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get laptops successful",
                Data = _mapper.Map<List<LaptopResponse>>(result)
            };

            return response;
        }

        public async Task<LaptopObjectResponse> UpdateAsync(int id, LaptopRequest laptop)
        {
            var existingLaptop = await _storeContext.Laptops
                .Include(l => l.Brand)
                .Include(l => l.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingLaptop == null)
            {
                throw new NotFoundException("Laptop not found");
            }

            _mapper.Map(laptop, existingLaptop);


            await _storeContext.SaveChangesAsync();

            var updatedLaptop = _mapper.Map<LaptopResponse>(existingLaptop); // Map sang LaptopResponse

            var response = new LaptopObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Laptop updated successfully",
                Data = updatedLaptop
            };

            return response;
        }
    }
}

