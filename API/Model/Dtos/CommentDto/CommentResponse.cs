using API.Services.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.CommentDto
{
    public class CommentResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [DisplayName("Nội dung")]
        public string Content { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }
        public bool IsDeleted { get; set; } = false;

        public bool IsQuestion { get; set; } = false;

        [DisplayName("Ngày đánh giá")]
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class CommentObjectResponse : ObjectResponse<CommentResponse> { }

    public class CommentListObjectResponse : ObjectResponse<List<CommentResponse>> { }
}
