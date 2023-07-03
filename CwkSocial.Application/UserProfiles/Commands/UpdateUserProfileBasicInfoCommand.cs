using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Commands;

public class UpdateUserProfileBasicInfoCommand : IRequest
{
    public Guid UserProfileId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string CurrentCity { get; set; }
}