using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, OperationResult<Post>>
{
    private readonly DataContext _context;

    public DeletePostCommandHandler(DataContext context)
    {
        _context = context;
    }
    
    public async Task<OperationResult<Post>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Post>();
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(post => post.PostId == request.PostId);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, 
                    string.Format(PostsErrorMessages.PostNotFound, request.PostId));
                return result;
            }

             if(post.UserProfileId != request.UserProfileId)
            {
                result.AddError(ErrorCode.PostDeleteNotPossible, PostsErrorMessages.PostDeleteNotPossible);
                return result;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            result.Payload = post;
        }
        catch (Exception e)
        {
           result.AddUnknownError(e.Message);
        }
        
        return result;
    }
}