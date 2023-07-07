using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class AddPostCommentCommandHandler : IRequestHandler<AddPostCommentCommand, OperationResult<PostComment>>
{
    private readonly DataContext _context;

    public AddPostCommentCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<PostComment>> Handle(AddPostCommentCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostComment>();
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(post => post.PostId == request.PostId);

            if (post is null)
            {
                result.isError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No Post with ID {request.PostId}" };
                result.Errors.Add(error);
                return result;
            }

            var comment = PostComment.CreatePostComment(request.PostId, request.Text, request.UserProfileId);
            post.AddPostComment(comment);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            result.Payload = comment;
        }
        catch (PostCommentNotValidException ex)
        {
            result.isError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                var error = new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = $"{ex.Message}",
                };
                result.Errors.Add(error);
            });
        }
        catch (Exception ex)
        {
            result.isError = true;
            var error = new Error { Code = ErrorCode.UnknownError, Message = ex.Message };
            result.Errors.Add(error);
        }

        return result;
    }
}