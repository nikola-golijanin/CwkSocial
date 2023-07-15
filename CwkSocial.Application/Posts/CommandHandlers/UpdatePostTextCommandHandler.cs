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
                result.IsError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No Post with ID {request.PostId}" };
                result.Errors.Add(error);
                return result;
            }

            post.UpdatePostText(request.TextContent);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            result.Payload = post;
        }
        catch (PostNotValidException ex)
        {
            result.IsError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                var error = new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = $"{ex.Message}",
                };
                result.Errors.Add(error);
            });
        }
        catch (Exception e)
        {
            var error = new Error { Code = ErrorCode.InternalServerError, Message = e.Message };
            result.IsError = true;
            result.Errors.Add(error);
        }

        return result;
    }
}