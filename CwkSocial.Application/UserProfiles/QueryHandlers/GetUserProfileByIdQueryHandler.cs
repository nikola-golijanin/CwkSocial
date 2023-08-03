using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.QueryHandlers;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public GetUserProfileByIdQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(GetUserProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(profile => profile.UserProfileId == request.UserProfileId);
        var result = new OperationResult<UserProfile>();

        if (userProfile is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(UserProfilesErrorMessages.UserProfileNotFound, request.UserProfileId));
            return result;
        }

        result.Payload = userProfile;
        return result;
    }
}