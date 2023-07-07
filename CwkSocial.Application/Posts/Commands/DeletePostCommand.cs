using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Commands;

public class DeletePostCommand : IRequest<OperationResult<Post>>
{
    public Guid PostId { get; set; }
}