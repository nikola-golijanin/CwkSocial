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
                result.AddError(ErrorCode.NotFound, 
                    string.Format(PostsErrorMessages.PostNotFound, request.PostId));
                return result;
            }
            
            var comment = post.Comments
                .FirstOrDefault(c => c.CommentId == request.CommentId);
            
            if (comment is null)
            {
                result.AddError(ErrorCode.NotFound, 
                    string.Format(PostsErrorMessages.CommentNotFound, request.CommentId));
                return result;
            }
            

            if (comment.UserProfileId != request.UserProfileId)
            {
                _result.AddError(ErrorCode.CommentRemovalNotAuthorized, 
                    PostsErrorMessages.CommentRemovalNotAuthorized);
                return _result;
            }
            
            comment.UpdateCommentText(request.UpdatedText);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            result.Payload = comment;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }
}