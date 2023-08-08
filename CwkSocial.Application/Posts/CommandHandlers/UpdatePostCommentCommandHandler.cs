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
    private readonly OperationResult<PostComment> _result;

    public UpdatePostCommentCommandHandler(DataContext context)
    {
        _context = context;
        _result = new OperationResult<PostComment>();

    }

    public async Task<OperationResult<PostComment>> Handle(UpdatePostCommentCommand request, CancellationToken cancellationToken)
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

        comment.UpdateCommentText(request.UpdatedText);
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);

        return _result;
    }
}