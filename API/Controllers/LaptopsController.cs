using API.Data;
using API.Helpers;
using API.Model.Dtos.LaptopDto;
using API.Model.Entity;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Get all laptops
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //// GET: api/Laptops
        [HttpGet]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetLaptops([FromQuery] int pageNumber = 1, int pageSize = 5)
        {

            var result = await _laptopRepository.GetAsync(pageNumber, pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Get laptop by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Laptops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Laptop>> GetLaptop(int id)
        {
            var result = await _laptopRepository.GetIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Search for laptop
        /// </summary>
        /// <param name="brandQuery"></param>
        /// <param name="priceSortOrder"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("search-laptops")]
        public async Task<IActionResult> GetLaptops([FromQuery] string? brandQuery, string? priceSortOrder, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _laptopRepository.SearchAsync(brandQuery, priceSortOrder, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Update laptop by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        //// PUT: api/Laptops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLaptop(int id, [FromBody] LaptopRequest request)
        {
            var result = await _laptopRepository.UpdateAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Create a new laptop
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="brandId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Laptops
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]  
        public async Task<ActionResult<LaptopObjectResponse>> PostLaptop([FromQuery] int categoryId, int brandId, [FromBody] LaptopRequest request)
        {
            var laptop = _mapper.Map<Laptop>(request);

            var result = await _laptopRepository.CreateAsync(categoryId, brandId, laptop);

            return CreatedAtAction("GetLaptop", new { id = laptop.Id }, result);
        }

        /// <summary>
        /// Delete laptop by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
