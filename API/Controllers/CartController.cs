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

        /// <summary>
        /// Get cart by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var result = await _cartRepository.GetCartAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Add to cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddToCart(string userId, int productId, int quantity)
        {
            var result = await _cartRepository.AddAsync(userId, productId, quantity);

            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }

        /// <summary>
        /// Decrease cart item
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpDelete("Decrease")]
        public async Task<IActionResult> DecreaseCartItem(string userId, int productId, int quantity)
        {
            var result = await _cartRepository.DecreaseAsync(userId, productId, quantity);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }

        /// <summary>
        /// Remove cart item
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveCartItem(string userId, int productId)
        {
            var result = await _cartRepository.RemoveAsync(userId, productId);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }

        /// <summary>
        /// Remove all cart items
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("RemoveAll")]
        public async Task<IActionResult> RemoveAllCartItem(string userId)
        {
            var result = await _cartRepository.RemoveAllAsync(userId);
            return CreatedAtAction(nameof(GetCart), new { userId = userId }, result);
        }
    }
}
