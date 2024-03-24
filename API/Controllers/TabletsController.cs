using API.Data;
using API.Model.Dtos.TabletDto;
using API.Model.Entity;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabletsController : ControllerBase
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly ITabletRepository _tabletRepository;

        public TabletsController(StoreContext storeContext, IMapper mapper, ITabletRepository tabletRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _tabletRepository = tabletRepository;
        }

        // GET: api/Tablets
        [HttpGet]
        public async Task<IActionResult> GetTablets()
        {
            var tablets = await _storeContext.Products.OfType<Tablet>()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(tablets);
        }

        //GET: api/Tablets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tablet>> GetTablet(int id)
        {
            var tablet = await _storeContext.Tablets
                .Include(t => t.Category)
                .Include(t => t.Brand)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tablet == null)
            {
                return NotFound();
            }

            return tablet;
        }

        // PUT: api/Tablets/5
        [HttpPut("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutTablet(int id, [FromBody] TabletRequest request)
        {
            var result = await _tabletRepository.UpdateAsync(id, request);
            return Ok(result);
        }

        // POST: api/Tablets
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<TabletObjectResponse>> PostTablet([FromQuery] int categoryId, int brandId, [FromBody] TabletRequest request)
        {
            var tablet = _mapper.Map<Tablet>(request);

            var result = await _tabletRepository.CreateAsync(categoryId, brandId, tablet);

            return CreatedAtAction("GetTablet", new { id = tablet.Id }, result);
        }

        [HttpGet("search-tablets")]
        public async Task<IActionResult> GetLaptops([FromQuery] string? brandQuery, string? priceSortOrder, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tabletRepository.SearchAsync(brandQuery, priceSortOrder, pageNumber, pageSize);
            return Ok(result);
        }

        // DELETE: api/Tablets/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteTablet(int id)
        {
            var result = await _tabletRepository.DeleteAsync(id);
            return Ok(result);
        }
    }
}
