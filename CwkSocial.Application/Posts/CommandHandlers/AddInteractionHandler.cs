using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class AddInteractionHandler : IRequestHandler<AddInteraction, OperationResult<PostInteraction>>
{
    private readonly DataContext _context;

    public AddInteractionHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<PostInteraction>> Handle(AddInteraction request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<PostInteraction>();
        try
        {
            var post = await _context.Posts.Include(p => p.Interactions)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostNotFound);
                return result;
            }

            var interaction = PostInteraction.CreatePostInteraction(request.PostId, request.UserProfileId,
                request.Type);

            post.AddInteraction(interaction);

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            result.Payload = interaction;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }
}