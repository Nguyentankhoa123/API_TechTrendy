using API.Model.Dtos.CommentDto;

namespace API.Repository
{
    public interface ICommentRepository
    {
        Task<CommentObjectResponse> CreateComment(CommentRequest request);

        Task<CommentListObjectResponse> GetCommentsByParentId(int? parentId, int productId);

        Task<CommentObjectResponse> DeleteComments(int commentId, int productId);
    }
}
