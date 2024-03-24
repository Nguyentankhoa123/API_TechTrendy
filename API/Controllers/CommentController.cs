using API.Model.Dtos.CommentDto;
using API.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        // GET: api/<CommentController>
        //[HttpGet]
        //public List<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<CommentController>/5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? parentId, int productId)
        {
            var result = await _commentRepository.GetCommentsByParentId(parentId, productId);
            return Ok(result);
        }

        // POST api/<CommentController>
        [HttpPost]
        public async Task<IActionResult> Post(CommentRequest request)
        {
            var result = await _commentRepository.CreateComment(request);
            return Ok(result);
        }

        // PUT api/<CommentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CommentController>/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCommentRequest request)
        {
            var result = await _commentRepository.DeleteComments(request.CommentId, request.ProductId);
            return Ok(result);
        }
    }
}
