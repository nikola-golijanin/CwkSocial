using CwkSocial.Domain.Aggregate.UserProfileAggregate;

namespace CwkSocial.Domain.Aggregate.PostAggregate;

public class Post
{
    private readonly List<PostComment> _comments = new();
    private readonly List<PostInteraction> _interactions = new();

    private Post()
    {
    }

    public Guid PostId { get; private set; }
    public Guid UserProfileId { get; private set; }
    public UserProfile UserProfile { get; private set; }
    public string TextContent { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime LastModified { get; private set; }

    public IEnumerable<PostComment> Comments
    {
        get { return _comments; }
    }

    public IEnumerable<PostInteraction> Interactions
    {
        get { return _interactions; }
    }


    //factories
    public static Post CreatePost(Guid userProfileId, string textContent)
    {
        //TODO: add validation, error handling strategies, error notification strategies
        return new Post
        {
            UserProfileId = userProfileId,
            TextContent = textContent,
            DateCreated = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
    }

    //public methods
    public void UpdatePostText(string newText)
    {
        TextContent = newText;
        LastModified = DateTime.UtcNow;
    }

    public void AddPostComment(PostComment newComment)
    {
        _comments.Add(newComment);
    }

    public void RemovePostComment(PostComment commentToRemove)
    {
        _comments.Remove(commentToRemove);
    }

    public void AddInteraction(PostInteraction newInteraction)
    {
        _interactions.Add(newInteraction);
    }

    public void RemoveInteraction(PostInteraction interactionToRemove)
    {
        _interactions.Remove(interactionToRemove);
    }
}