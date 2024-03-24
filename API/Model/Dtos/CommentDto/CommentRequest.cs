namespace API.Model.Dtos.CommentDto
{
    public class CommentRequest
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? ParentId { get; set; }
    }

    public class DeleteCommentRequest
    {
        public int CommentId { get; set; }
        public int ProductId { get; set; }
    }
}
