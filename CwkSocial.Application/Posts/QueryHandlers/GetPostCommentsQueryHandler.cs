using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class
    GetPostCommentsQueryHandler : IRequestHandler<GetPostCommentsQuery, OperationResult<IEnumerable<PostComment>>>
{
    private readonly DataContext _context;

    public GetPostCommentsQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<IEnumerable<PostComment>>> Handle(GetPostCommentsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<PostComment>>();
        try
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId);
            
            if (post is null)
            {
                result.AddError(ErrorCode.NotFound,
                    string.Format(PostsErrorMessages.PostNotFound, request.PostId));
            }

            result.Payload = post.Comments;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }
}