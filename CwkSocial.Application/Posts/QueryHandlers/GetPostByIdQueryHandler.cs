using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Posts.QueryHandlers;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, OperationResult<Post>>
{
    private readonly DataContext _context;

    public GetPostByIdQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Post>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(post => post.PostId == request.PostId);
        var result = new OperationResult<Post>();

        if (post is null)
        {
            result.isError = true;
            var error = new Error
                { Code = ErrorCode.NotFound, Message = $"No Post with ID {request.PostId}" };
            result.Errors.Add(error);
            return result;
        }

        result.Payload = post;
        return result;
    }
}