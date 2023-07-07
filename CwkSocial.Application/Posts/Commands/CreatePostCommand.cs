using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class CreatePostCommand : IRequest<OperationResult<Post>>
{
    public Guid UserProfileId { get; set; }
    public string TextContent { get; set; }
}