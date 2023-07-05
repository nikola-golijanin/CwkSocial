using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using FluentValidation;

namespace CwkSocial.Domain.Validators.UserProfileValidators;

public class BasicInfoValidator : AbstractValidator<BasicInfo>
{
    public BasicInfoValidator()
    {
        RuleFor(info => info.FirstName)
            .NotNull().WithMessage("First name is required. It is currently null")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("First name can contain at most 50 characters long.");

        RuleFor(info => info.LastName)
            .NotNull().WithMessage("Last name is required. It is currently null")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Last name can contain at most 50 characters long.");

        RuleFor(info => info.EmailAddress)
            .NotNull().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Email address is not in required email format");

        RuleFor(info => info.DateOfBirth)
            .InclusiveBetween(
                from: new DateTime(DateTime.Now.AddYears(-80).Ticks),
                to: new DateTime(DateTime.Now.AddYears(-18).Ticks))
            .WithMessage("Age needs to be between 18 and 80");
    }
}