using System.ComponentModel.DataAnnotations;
using CwkSocial.Domain.Aggregate.PostAggregate;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class PostInteractionCreate
{
    [Required]
    public InteractionType Type { get; set; }
}