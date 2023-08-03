using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class UpdatePostTextCommandHandler : IRequestHandler<UpdatePostTextCommand, OperationResult<Post>>
{
    private readonly DataContext _context;

    public UpdatePostTextCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Post>> Handle(UpdatePostTextCommand request, CancellationToken cancellationToken)
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
                result.AddError(ErrorCode.PostUpdateNotPossible, PostsErrorMessages.PostUpdateNotPossible);
                return result;
            }

            post.UpdatePostText(request.TextContent);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            result.Payload = post;
        }
        catch (PostNotValidException ex)
        {
            ex.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception ex)
        {
            result.AddError(ErrorCode.UnknownError, ex.Message);
        }

        return result;
    }
}