using API.Data;
using API.Model.Dtos.DiscountDto;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public DiscountController(IDiscountRepository discountRepository, StoreContext storeContext, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _storeContext = storeContext;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all discounts
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string code, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _discountRepository.GetAllDiscountWithProduct(code, pageNumber, pageSize);
            return Ok(result);
        }
        /// <summary>
        /// Get discount by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        // GET api/<DiscountController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var result = await _discountRepository.GetAllDiscountsForUser(userId);
            return Ok(result);
        }
        /// <summary>
        /// Create a new discount
        /// </summary>
        /// <param name="request"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        // POST api/<DiscountController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DiscountRequest request, [FromQuery] List<int>? productId)
        {
            var result = await _discountRepository.CreateDiscountCode(request, productId);
            return Ok(result);
        }
    }
}
