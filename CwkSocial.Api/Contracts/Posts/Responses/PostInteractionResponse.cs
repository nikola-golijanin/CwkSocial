namespace CwkSocial.Api.Contracts.Posts.Responses;

public class PostInteractionResponse
{
    public string Type { get; set; }
    public Guid InteractionId { get; set; }
    public InteractionUser Author { get; set; }
}

