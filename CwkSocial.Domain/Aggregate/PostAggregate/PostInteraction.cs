﻿namespace CwkSocial.Domain.Aggregate.PostAggregate;

public class PostInteraction
{
    private PostInteraction()
    {
    }

    public Guid InteractionId { get; private set; }
    public Guid PostId { get; private set; }
    public InteractionType InteractionType { get; private set; }

    //factories
    public static PostInteraction CreatePostInteraction(Guid postId, InteractionType interactionType)
    {
        //TODO: add validation, error handling strategies, error notification strategies
        return new PostInteraction
        {
            PostId = postId,
            InteractionType = interactionType
        };
    }
}