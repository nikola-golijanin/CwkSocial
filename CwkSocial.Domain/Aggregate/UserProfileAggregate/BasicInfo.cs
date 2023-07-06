using CwkSocial.Domain.Exceptions;
using CwkSocial.Domain.Validators.UserProfileValidators;

namespace CwkSocial.Domain.Aggregate.UserProfileAggregate;

public class BasicInfo
{
    private BasicInfo()
    {
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string Phone { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string CurrentCity { get; private set; }

    /// <summary>
    ///  Create basic info for user profile
    /// </summary>
    /// <param name="firstName">First name of user profile</param> 
    /// <param name="lastName"> Last name of user profile</param>
    /// <param name="emailAddress">Email of user profile</param> 
    /// <param name="phone">User profile phone</param> 
    /// <param name="dateOfBirth">User profile date of birth</param> 
    /// <param name="currentCity">User profile current city</param> 
    /// <returns><see cref="BasicInfo"/></returns>
    /// <exception cref="UserProfileNotValidException">Thrown if data provided for the basic info is not valid.</exception>
    public static BasicInfo CreateBasicInfo(string firstName, string lastName, string emailAddress, string phone,
        DateTime dateOfBirth, string currentCity)
    {
        var validator = new BasicInfoValidator();

        var objectToValidate = new BasicInfo
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = emailAddress,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            CurrentCity = currentCity
        };

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new UserProfileNotValidException("The user profile is not valid");
        foreach (var error in validationResult.Errors)
        {
            exception.ValidationErrors.Add(error.ErrorMessage);
        }

        throw exception;
    }
}