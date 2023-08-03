using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, OperationResult<IEnumerable<Post>>>
{
    private readonly DataContext _context;

    public GetAllPostsQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<IEnumerable<Post>>> Handle(GetAllPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<Post>>();
        try
        {
            result.Payload = await _context.Posts.ToListAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}