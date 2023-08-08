using CwkSocial.Domain.Aggregate.PostAggregate;
using CwkSocial.Application.Models;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;


namespace CwkSocial.Application.Posts.CommandHandlers;

public class RemovePostInteractionHandler : IRequestHandler<RemovePostInteraction, OperationResult<PostInteraction>>
{
    private readonly DataContext _context;

    public RemovePostInteractionHandler(DataContext context)
    {
        _context = context;
    }
    public async Task<OperationResult<PostInteraction>> Handle(RemovePostInteraction request, 
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostInteraction>();
        try
        {
            var post = await _context.Posts
                .Include(p => p.Interactions)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound,
                    string.Format(PostsErrorMessages.PostNotFound, request.PostId));
                return result;
            }

            var interaction = post.Interactions
                .FirstOrDefault(i => i.InteractionId == request.InteractionId);

            if (interaction == null)
            {
                result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostInteractionNotFound);
                return result;
            }

            if (interaction.UserProfileId != request.UserProfileId)
            {
                result.AddError(ErrorCode.InteractionRemovalNotAuthorized,
                    PostsErrorMessages.InteractionRemovalNotAuthorized);
                return result;
            }

            post.RemoveInteraction(interaction);
            _context.Posts.Update(post);
            await _ctx.SaveChangesAsync(cancellationToken);

            result.Payload = interaction;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}