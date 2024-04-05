using API.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public ImagesController(ICloudinaryRepository cloudinaryRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
        }
        /// <summary>
        /// Upload image
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            var folderName = "E-Commerce";
            var urls = await _cloudinaryRepository.UploadImagesAsync(files, folderName);

            return Ok(new { urls = urls });
        }
    }
}
