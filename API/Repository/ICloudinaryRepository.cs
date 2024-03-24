namespace API.Repository
{
    public interface ICloudinaryRepository
    {
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder);
    }
}
