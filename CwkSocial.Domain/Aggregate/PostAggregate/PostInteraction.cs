﻿using CwkSocial.Domain.Aggregate.UserProfileAggregate;

namespace CwkSocial.Domain.Aggregate.PostAggregate;

public class PostInteraction
{
    private PostInteraction()
    {
    }

    public Guid InteractionId { get; private set; }
    public Guid PostId { get; private set; }
    public InteractionType InteractionType { get; private set; }
    public Guid? UserProfileId { get; private set; }
    public UserProfile UserProfile { get; private set; }

    //factories
    public static PostInteraction CreatePostInteraction(Guid postId, Guid userProfileId, InteractionType interactionType)
    {
        return new PostInteraction
        {
            PostId = postId,
            InteractionType = interactionType,
            UserProfileId = userProfileId
        };
    }
}