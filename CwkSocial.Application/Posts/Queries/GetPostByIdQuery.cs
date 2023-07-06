﻿using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.PostAggregate;
using MediatR;

namespace CwkSocial.Application.Posts.Queries;

public class GetPostByIdQuery : IRequest<OperationResult<Post>>
{
    public Guid PostId { get; set; }
}