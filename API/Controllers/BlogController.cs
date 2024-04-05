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

        /// <summary>
        /// Get all blog
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber, int pageSize)
        {
            var result = await _blogRepository.GetAllBlog(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get blog by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _blogRepository.GetBlog(id);
            return Ok(result);
        }

        /// <summary>
        /// Craete a new blog
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BlogRequest request, [FromQuery] string userId)
        {
            var result = await _blogRepository.CreateBlog(request, userId);
            return Ok(result);
        }

        /// <summary>
        /// Update blog by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BlogRequest request)
        {
            var result = await _blogRepository.UpdateBlog(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Delete blog by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _blogRepository.DeleteBlog(id);
            return Ok(result);
        }
    }
}
