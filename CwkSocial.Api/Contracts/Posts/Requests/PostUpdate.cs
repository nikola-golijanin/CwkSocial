using System.ComponentModel.DataAnnotations;

namespace CwkSocial.Api.Contracts.Posts.Requests;

public class PostUpdate
{
    [Required]
    public string TextContent { get; set; }
}