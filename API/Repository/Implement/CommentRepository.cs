using API.Data;
using API.Model.Dtos.CommentDto;
using API.Model.Entity;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class CommentRepository : ICommentRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public CommentRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }
        public async Task<CommentObjectResponse> CreateComment(CommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            int rightValue = 0;
            if (request.ParentId.HasValue)
            {
                // reply comment
                var parentComment = await _storeContext.Comments.FindAsync(request.ParentId.Value);
                if (parentComment == null)
                {
                    throw new NotFoundException("Parent comment not found");
                }

                rightValue = parentComment.Right;
                // updateMany comments

                var commentsToUpdateRight = _storeContext.Comments
                    .Where(c => c.ProductId == request.ProductId && c.Right >= rightValue);

                foreach (var commentToUpdate in commentsToUpdateRight)
                {
                    commentToUpdate.Right += 2;
                }

                var commentsToUpdateLeft = _storeContext.Comments
                    .Where(c => c.ProductId == request.ProductId && c.Left > rightValue);

                foreach (var commentToUpdate in commentsToUpdateLeft)
                {
                    commentToUpdate.Left += 2;
                }

                await _storeContext.SaveChangesAsync();
            }
            else
            {

                var maxRightValue = await _storeContext.Comments
                    .Where(x => x.ProductId == request.ProductId)
                    .MaxAsync(x => (int?)x.Right);
                if (maxRightValue != null)
                {
                    rightValue = maxRightValue.Value + 1;
                }
                else
                {
                    rightValue = 1;
                }
            }
            comment.Left = rightValue;
            comment.Right = rightValue + 1;

            _storeContext.Comments.Add(comment);
            await _storeContext.SaveChangesAsync();

            var response = new CommentObjectResponse
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Comment created",
                Data = _mapper.Map<CommentResponse>(comment)
            };

            return response;
        }

        public async Task<CommentObjectResponse> DeleteComments(int commentId, int productId)
        {
            var foundProduct = await _storeContext.Products.FindAsync(productId);

            if (foundProduct == null)
            {
                throw new NotFoundException("Product not found");
            }

            var comment = await _storeContext.Comments.FindAsync(commentId);

            var leftValue = comment.Left;
            var rightValue = comment.Right;

            var width = rightValue - leftValue + 1;

            var commentsToDelete = _storeContext.Comments
                .Where(c => c.ProductId == productId && c.Left >= leftValue && c.Right <= rightValue);

            _storeContext.Comments.RemoveRange(commentsToDelete);

            var commentsToUpdateRight = _storeContext.Comments
                .Where(c => c.ProductId == productId && c.Right > rightValue);

            foreach (var commentToUpdate in commentsToUpdateRight)
            {
                commentToUpdate.Right -= width;
            }

            var commentsToUpdateLeft = _storeContext.Comments
                .Where(c => c.ProductId == productId && c.Left > rightValue);

            foreach (var commentToUpdate in commentsToUpdateLeft)
            {
                commentToUpdate.Left -= width;
            }

            await _storeContext.SaveChangesAsync();

            return new CommentObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Comments deleted"
            };
        }

        public async Task<CommentListObjectResponse> GetCommentsByParentId(int? parentId, int productId)
        {
            if (parentId.HasValue)
            {
                var parent = await _storeContext.Comments.FindAsync(parentId.Value);
                if (parent == null)
                {
                    throw new NotFoundException("Not found comment for product");
                }

                var comments = await _storeContext.Comments
                    .Where(c => c.ProductId == productId && c.Left > parent.Left && c.Right <= parent.Right)
                    .OrderBy(c => c.Left)
                    .ToListAsync();

                var response = new CommentListObjectResponse
                {
                    StatusCode = ResponseCode.OK,
                    Message = "Get comment",
                    Data = _mapper.Map<List<CommentResponse>>(comments)
                };

                return response;
            }
            else
            {
                var comments = await _storeContext.Comments
                    .Where(c => c.ProductId == productId && c.ParentId == parentId)
                    .OrderBy(c => c.Left)
                    .ToListAsync();

                var response = new CommentListObjectResponse
                {
                    StatusCode = ResponseCode.OK,
                    Message = "Get comment",
                    Data = _mapper.Map<List<CommentResponse>>(comments)
                };

                return response;
            }
        }
    }
}


