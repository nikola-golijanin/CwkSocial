using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class UpdatePostCommentCommandHandler
    : IRequestHandler<UpdatePostCommentCommand, OperationResult<PostComment>>
{
    
    private readonly DataContext _context;

    public UpdatePostCommentCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<PostComment>> Handle(UpdatePostCommentCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostComment>();

        try
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId);
            
            if (post is null)
            {
                result.isError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No Post with ID {request.PostId}" };
                result.Errors.Add(error);
                return result;
            }
            
            var comment = post.Comments
                .FirstOrDefault(c => c.CommentId == request.CommentId);
            
            if (comment is null)
            {
                result.isError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No Post with ID {request.PostId}" };
                result.Errors.Add(error);
                return result;
            }
            
            //TODO check for userProfile
            
            comment.UpdateCommentText(request.UpdatedText);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            result.Payload = comment;
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