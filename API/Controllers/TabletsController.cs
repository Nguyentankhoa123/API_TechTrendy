using API.Data;
using API.Model.Dtos.TabletDto;
using API.Model.Entity;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
        /// <summary>
        /// Get all tablets
        /// </summary>
        /// <returns></returns>
        // GET: api/Tablets
        [HttpGet]
        public async Task<IActionResult> GetTablets([FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _tabletRepository.GetAsync(pageNumber, pageSize);
            return Ok(result);
        }
        /// <summary>
        /// Get a tablet by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //GET: api/Tablets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tablet>> GetTablet(int id)
        {
            var result = await _tabletRepository.GetIdAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Update tablet by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Tablets/5
        [HttpPut("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutTablet(int id, [FromBody] TabletRequest request)
        {
            var result = await _tabletRepository.UpdateAsync(id, request);
            return Ok(result);
        }
        /// <summary>
        /// Create a new tablet
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="brandId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Tablets
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<TabletObjectResponse>> PostTablet([FromQuery] int categoryId, int brandId, [FromBody] TabletRequest request)
        {
            var tablet = _mapper.Map<Tablet>(request);

            var result = await _tabletRepository.CreateAsync(categoryId, brandId, tablet);

            return CreatedAtAction(nameof(GetTablet), new { id = tablet.Id }, result);
        }
        /// <summary>
        /// Search for tablet
        /// </summary>
        /// <param name="brandQuery"></param>
        /// <param name="priceSortOrder"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("search-tablets")]
        public async Task<IActionResult> GetTablets([FromQuery] string? brandQuery, string? priceSortOrder, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tabletRepository.SearchAsync(brandQuery, priceSortOrder, pageNumber, pageSize);
            return Ok(result);
        }
        /// <summary>
        /// Delete tablet by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
