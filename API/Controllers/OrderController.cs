﻿using API.Data;
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
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var order = await _orderRepository.GetOrderAsync(id);

            return Ok(order);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderRequest orderDto, string userId, string? code)
        {
            var result = await _orderRepository.CreateOrderAsync(orderDto, userId, code, HttpContext);
            return CreatedAtAction(nameof(Get), new { userId = userId }, result);
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}