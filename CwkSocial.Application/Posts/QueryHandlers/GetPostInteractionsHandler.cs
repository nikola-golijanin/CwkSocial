using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetPostInteractionsHandler : IRequestHandler<GetPostInteractions, OperationResult<IEnumerable<PostInteraction>>>
{
    private readonly DataContext _context;

    public GetPostInteractionsHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<IEnumerable<PostInteraction>>> Handle(GetPostInteractions request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<PostInteraction>>();
        try
        {
            var post = await _context.Posts
                .Include(p => p.Interactions)
                .ThenInclude(i => i.UserProfile)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId);

            if (post is null)
            {
                result.AddError(ErrorCode.NotFound, PostsErrorMessages.PostNotFound);
                return result;
            }

            result.Payload = post.Interactions;

        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }

}