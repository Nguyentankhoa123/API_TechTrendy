using API.Data;
using API.Model.Dtos.OrderDto;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, StoreContext storeContext, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _storeContext = storeContext;
            _mapper = mapper;
        }
        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var order = await _orderRepository.GetOrderAsync(id);

            return Ok(order);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="orderDto"></param>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        // POST api/<OrderController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderRequest orderDto, string userId, string? code, int addressId)
        {
            var result = await _orderRepository.CreateOrderAsync(orderDto, userId, code, addressId, HttpContext);
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        }
    }
}
