using CwkSocial.Application.Models;
using MediatR;
using CwkSocial.Domain.Aggregate.PostAggregate;

namespace CwkSocial.Application.Posts.Commands;

public class RemoveCommentFromPost : IRequest<OperationResult<PostComment>>
{
    public Guid UserProfileId { get; set; }
    public Guid PostId { get; set; }
    public Guid CommentId { get; set; }
}