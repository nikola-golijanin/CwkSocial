using CwkSocial.Domain.Aggregate.PostAggregate;
using FluentValidation;

namespace CwkSocial.Domain.Validators.PostsValidators;

public class PostCommentValidator : AbstractValidator<PostComment>
{
    public PostCommentValidator()
    {
        RuleFor(comment => comment.Text)
            .NotNull().WithMessage("Comment content should not be null")
            .NotEmpty().WithMessage("Comment content should not be empty")
            .MinimumLength(1).WithMessage("Minimum length of comment must be at least 1 character long.")
            .MaximumLength(500).WithMessage("Maximum length of comment can be at most 500 characters long.");
    }
}