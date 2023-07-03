using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetUserProfileByIdQuery : IRequest<UserProfile>
{
    public Guid UserProfileId { get; set; }
}