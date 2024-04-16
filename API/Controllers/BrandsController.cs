using API.Data;
using API.Helpers;
using API.Model.Dtos.BrandDto;
using API.Model.Entity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;

        public BrandsController(StoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all brands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands(int pageNumber = 1, int pageSize = 5)
        {
            var brands = await _context.Brands
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!brands.Any())
            {
                return NotFound("No brands found.");
            }

            return Ok(brands);
        }

        /// <summary>
        /// Get brand by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        /// <summary>
        /// Create a new brand
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<BrandObjectResponse>> PostBrand(BrandRequest request)
        {
            var brand = _mapper.Map<Brand>(request);
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return Ok(brand);
        }

        /// <summary>
        /// Delete brand by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
