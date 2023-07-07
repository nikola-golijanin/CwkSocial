using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class AddPostCommentCommand : IRequest<OperationResult<PostComment>>
{
    public Guid PostId { get; set; }
    public Guid UserProfileId { get; set; }
    public string Text { get; set; }
}