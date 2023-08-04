using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Queries;

public class GetPostInteractions : IRequest<OperationResult<IEnumerable<PostInteraction>>>
{
    public Guid PostId { get; set; }
}