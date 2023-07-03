using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetAllUserProfilesQuery : IRequest<IEnumerable<UserProfile>>
{
}