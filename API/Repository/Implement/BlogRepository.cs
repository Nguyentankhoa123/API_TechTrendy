using API.Data;
using API.Model.Dtos.BlogDto;
using API.Model.Entity;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class BlogRepository : IBlogRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public BlogRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }

        public async Task<BlogObjectResponse> CreateBlog(BlogRequest request, string userId)
        {
            var user = await _storeContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var blog = _mapper.Map<Blog>(request);
            blog.UserId = userId;
            _storeContext.Blogs.Add(blog);
            await _storeContext.SaveChangesAsync();

            var response = new BlogObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Blog created successfully.",
                Data = _mapper.Map<BlogResponse>(blog)
            };
            return response;
        }

        public async Task<BlogObjectResponse> DeleteBlog(int blogId)
        {
            var blog = await _storeContext.Blogs.FindAsync(blogId);

            if (blog == null)
            {
                throw new NotFoundException("Blog not found");
            };

            _storeContext.Blogs.Remove(blog);
            await _storeContext.SaveChangesAsync();

            var response = new BlogObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Blog deleted successfully."
            };

            return response;
        }

        public async Task<BlogListObjectResponse> GetAllBlog(int pageNumber, int pageSize)
        {
            var blogs = await _storeContext.Blogs
                .Include(b => b.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (blogs == null || !blogs.Any())
            {
                throw new NotFoundException("No blogs found.");
            };

            var response = new BlogListObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Retrieved all blogs successfully.",
                Data = _mapper.Map<List<BlogResponse>>(blogs)
            };

            return response;
        }

        public async Task<BlogObjectResponse> GetBlog(int blogId)
        {
            var blog = await _storeContext.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(x => x.Id == blogId);

            if (blog == null)
            {
                throw new NotFoundException("Blog not found");
            };

            var response = new BlogObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Retrieved blog successfully.",
                Data = _mapper.Map<BlogResponse>(blog)
            };

            return response;
        }

        public async Task<BlogObjectResponse> UpdateBlog(int blogId, BlogRequest request)
        {
            var blog = await _storeContext.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(x => x.Id == blogId);

            if (blog == null)
            {
                throw new NotFoundException("Blog not found");
            };

            _mapper.Map(request, blog);

            await _storeContext.SaveChangesAsync();

            var response = new BlogObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Blog updated successfully.",
                Data = _mapper.Map<BlogResponse>(blog)
            };

            return response;
        }
    }
}
