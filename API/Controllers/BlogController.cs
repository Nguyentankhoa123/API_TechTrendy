using API.Model.Dtos.BlogDto;
using API.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        // GET: api/<BlogController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _blogRepository.GetAllBlog();
            return Ok(result);
        }

        // GET api/<BlogController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _blogRepository.GetBlog(id);
            return Ok(result);
        }

        // POST api/<BlogController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BlogRequest request, [FromQuery] string userId)
        {
            var result = await _blogRepository.CreateBlog(request, userId);
            return Ok(result);
        }

        // PUT api/<BlogController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BlogRequest request)
        {
            var result = await _blogRepository.UpdateBlog(id, request);
            return Ok(result);
        }

        // DELETE api/<BlogController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _blogRepository.DeleteBlog(id);
            return Ok(result);
        }
    }
}
