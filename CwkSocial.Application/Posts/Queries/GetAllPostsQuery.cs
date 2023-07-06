using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Queries;

public class GetAllPostsQuery : IRequest<OperationResult<IEnumerable<Post>>>
{
}