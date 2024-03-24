using API.Data;
using API.Model.Dtos.LaptopDto;
using API.Model.Entity;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaptopsController : ControllerBase
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly ILaptopRepository _laptopRepository;

        public LaptopsController(StoreContext storeContext, IMapper mapper, ILaptopRepository laptopRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _laptopRepository = laptopRepository;
        }

        //// GET: api/Laptops
        [HttpGet]
        public async Task<IActionResult> GetLaptops()
        {
            //var laptops = await _context.Products.OfType<Laptop>()
            //    .Include(p => p.Brand)
            //    .Include(p => p.Category)
            //    .ToListAsync();

            //return Ok(laptops);

            var result = await _laptopRepository.GetAsync();

            return Ok(result);
        }

        // GET: api/Laptops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Laptop>> GetLaptop(int id)
        {
            var result = await _laptopRepository.GetIdAsync(id);
            return Ok(result);
        }

        [HttpGet("search-laptops")]
        public async Task<IActionResult> GetLaptops([FromQuery] string? brandQuery, string? priceSortOrder, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _laptopRepository.SearchAsync(brandQuery, priceSortOrder, pageNumber, pageSize);
            return Ok(result);
        }

        //// PUT: api/Laptops/5
        [HttpPut("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutLaptop(int id, [FromBody] LaptopRequest request)
        {
            var result = await _laptopRepository.UpdateAsync(id, request);
            return Ok(result);
        }

        // POST: api/Laptops

        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<LaptopObjectResponse>> PostLaptop([FromQuery] int categoryId, int brandId, [FromBody] LaptopRequest request)
        {
            var laptop = _mapper.Map<Laptop>(request);

            var result = await _laptopRepository.CreateAsync(categoryId, brandId, laptop);

            return CreatedAtAction("GetLaptop", new { id = laptop.Id }, result);
        }


        // DELETE: api/Laptops/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteLaptop(int id)
        {
            var result = await _laptopRepository.DeleteAsync(id);
            return Ok(result);
        }
    }
}
