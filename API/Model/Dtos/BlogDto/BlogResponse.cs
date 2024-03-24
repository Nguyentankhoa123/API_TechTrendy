using API.Services.Common;

namespace API.Model.Dtos.BlogDto
{
    public class BlogResponse
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        public string UserName { get; set; }
    }

    public class BlogObjectResponse : ObjectResponse<BlogResponse> { }

    public class BlogListObjectResponse : ObjectResponse<List<BlogResponse>> { }
}
