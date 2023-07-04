using CwkSocial.Application.Models;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;

namespace CwkSocial.Application.UserProfiles.Queries;

public class GetAllUserProfilesQuery : IRequest<OperationResult<IEnumerable<UserProfile>>>
{
}