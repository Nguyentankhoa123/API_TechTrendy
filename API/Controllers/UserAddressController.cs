using API.Data;
using API.Model.Dtos.User;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public UserAddressController(StoreContext storeContext, IMapper mapper, IAccountRepository accountRepository)
        {
            _storeContext = storeContext;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }
        /// <summary>
        /// Get the user address by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        // GET api/<UserAddressController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAddress(string userId)
        {
            var result = await _accountRepository.GetUserAddressAsync(userId);
            return Ok(result);
        }
        /// <summary>
        /// Create a new user address
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        // POST api/<UserAddressController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserAddressRequest request, [FromQuery] string userId)
        {
            var result = await _accountRepository.AddUserAddressAsync(request, userId);
            return CreatedAtAction(nameof(GetUserAddress), new { userId = userId }, result);
        }

        /// <summary>
        /// Update the user address
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        // PUT api/<UserAddressController>/5
        [HttpPut("{userId}")]
        public async Task<IActionResult> Put([FromBody] UserAddressRequest request, string userId, int addressId)
        {
            var result = await _accountRepository.UpdateUserAddressAsync(request, userId, addressId);
            return CreatedAtAction(nameof(GetUserAddress), new { userId = userId }, result);
        }
    }
}
