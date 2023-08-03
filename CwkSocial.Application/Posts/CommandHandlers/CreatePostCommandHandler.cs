using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.PostAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;

namespace CwkSocial.Application.Posts.CommandHandlers;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, OperationResult<Post>>
{
    private readonly DataContext _context;

    public CreatePostCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<Post>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<Post>();
        try
        {
            var post = Post.CreatePost(request.UserProfileId, request.TextContent);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            
            result.Payload = post;
        }
        catch (PostNotValidException ex)
        {
            ex.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }
}