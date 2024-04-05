using API.Model.Dtos.BlogDto;

namespace API.Repository
{
    public interface IBlogRepository
    {
        Task<BlogObjectResponse> CreateBlog(BlogRequest request, string UserId);

        Task<BlogObjectResponse> GetBlog(int blogId);

        Task<BlogListObjectResponse> GetAllBlog(int pageNumber, int pageSize);

        Task<BlogObjectResponse> UpdateBlog(int blogId, BlogRequest request);

        Task<BlogObjectResponse> DeleteBlog(int blogId);
    }
}
