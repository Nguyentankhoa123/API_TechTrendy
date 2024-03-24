using API.Services.Exceptions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Repository.Implement
{
    public class CloudinaryRepository : ICloudinaryRepository
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryRepository(IConfiguration configuration)
        {
            Account account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder)
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folder
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new BadRequestException(uploadResult.Error.Message);
                }

                urls.Add(uploadResult.SecureUrl.ToString());
            }

            return urls;
        }

    }
}
