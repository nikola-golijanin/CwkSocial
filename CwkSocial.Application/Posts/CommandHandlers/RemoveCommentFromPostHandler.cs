using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class RemoveCommentFromPostHandler 
    : IRequestHandler<RemoveCommentFromPost, OperationResult<PostComment>>
{
    private readonly DataContext _context;
    private readonly OperationResult<PostComment> _result;

    public RemoveCommentFromPostHandler(DataContext context)
    {
        _context = context;
        _result = new OperationResult<PostComment>();
    }

    public async Task<OperationResult<PostComment>> Handle(RemoveCommentFromPost request, 
        CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.PostId == request.PostId);

        if (post == null)
        {
            _result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostNotFound);
            return _result;
        }

        var comment = post.Comments
            .FirstOrDefault(c => c.CommentId == request.CommentId);
        if (comment == null)
        {
            _result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostCommentNotFound);
            return _result;
        }

        if (comment.UserProfileId != request.UserProfileId)
        {
            _result.AddError(ErrorCode.CommentRemovalNotAuthorized, 
                PostsErrorMessages.CommentRemovalNotAuthorized);
            return _result;
        }

        post.RemovePostComment(comment);
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);

        _result.Payload = comment;
        return _result;
    }
}