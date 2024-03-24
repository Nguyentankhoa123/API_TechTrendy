using API.Data;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;

        public CartController(StoreContext storeContext, IMapper mapper, ICartRepository cartRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _cartRepository = cartRepository;
        }

        // GET api/<CartController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var result = await _cartRepository.GetCartAsync(userId);
            return Ok(result);
        }

        // POST api/<CartController>
        [HttpPost]
        public async Task<IActionResult> AddToCart(string userId, int productId, int quantity)
        {
            var result = await _cartRepository.AddAsync(userId, productId, quantity);

            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }


        // DELETE api/<CartController>
        [HttpDelete("Decrease")]
        public async Task<IActionResult> DecreaseCartItem(string userId, int productId, int quantity)
        {
            var result = await _cartRepository.DecreaseAsync(userId, productId, quantity);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveCartItem(string userId, int productId)
        {
            var result = await _cartRepository.RemoveAsync(userId, productId);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }

        [HttpDelete("RemoveAll")]
        public async Task<IActionResult> RemoveAllCartItem(string userId)
        {
            var result = await _cartRepository.RemoveAllAsync(userId);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }
    }
}
