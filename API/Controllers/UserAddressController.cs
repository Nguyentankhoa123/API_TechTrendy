using API.Data;
using API.Model.Dtos.User;
using API.Model.Entity;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public UserAddressController(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }
        // GET: api/<UserAddressController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserAddressController>/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var user = _storeContext.UserAddresses
                .Include(u => u.User)
                .FirstOrDefault(x => x.UserId == userId);

            return Ok(user);
        }

        // POST api/<UserAddressController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserAddressDto userAddressDto, [FromQuery] string userId)
        {
            // Check if a UserAddress already exists for the user
            var existingAddress = await _storeContext.UserAddresses
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAddress != null)
            {
                return BadRequest("Địa chỉ đã tồn tại. Vui lòng cập nhật địa chỉ thay vì tạo mới.");
            }

            var address = _mapper.Map<UserAddress>(userAddressDto);
            address.UserId = userId;

            _storeContext.UserAddresses.Add(address);
            await _storeContext.SaveChangesAsync();

            var addressDto = _mapper.Map<UserAddressDto>(address);

            return Ok(addressDto);
        }


        // PUT api/<UserAddressController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserAddressController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
