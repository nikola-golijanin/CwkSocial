using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using CwkSocial.Domain.Validators.PostsValidators;

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
    /// <summary>
    /// Create post
    /// </summary>
    /// <param name="userProfileId">The ID of user who created the comment</param>
    /// <param name="textContent">Text content of comment</param>
    /// <returns><see cref="Post"/></returns>
    /// <exception cref="PostNotValidException">Thrown if data provided for the post is not valid</exception>
    public static Post CreatePost(Guid userProfileId, string textContent)
    {
        var validator = new PostValidator();
        var objectToValidate = new Post
        {
            UserProfileId = userProfileId,
            TextContent = textContent,
            DateCreated = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };

        var validationResult = validator.Validate(objectToValidate);
        if (validationResult.IsValid) return objectToValidate;
        var exception = new PostNotValidException("The Post is not valid");
        foreach (var error in validationResult.Errors)
        {
            exception.ValidationErrors.Add(error.ErrorMessage);
        }

        throw exception;
    }

    //public methods
    /// <summary>
    ///  Updates the post text
    /// </summary>
    /// <param name="newText">The updated post text</param>
    /// <exception cref="PostNotValidException">Thrown if new text is null or empty</exception>
    public void UpdatePostText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            var exception = new PostNotValidException("Cannot update post, new content is empty");
            exception.ValidationErrors.Add("The provided text is either null or whitespace");
            throw exception;
        }

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

    public void UpdatePostComment(Guid postCommentId, string updatedComment)
    {
        var comment = _comments.FirstOrDefault(c => c.CommentId == postCommentId);
        if (comment != null && !string.IsNullOrWhiteSpace(updatedComment))
            comment.UpdateCommentText(updatedComment);
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