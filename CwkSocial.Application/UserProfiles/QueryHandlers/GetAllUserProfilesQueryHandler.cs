using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Queries;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.QueryHandlers;

internal class
    GetAllUserProfilesQueryHandler : IRequestHandler<GetAllUserProfilesQuery, OperationResult<IEnumerable<UserProfile>>>
{
    private readonly DataContext _context;

    public GetAllUserProfilesQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<IEnumerable<UserProfile>>> Handle(GetAllUserProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<IEnumerable<UserProfile>>();
        result.Payload = await _context.UserProfiles.ToListAsync();
        return result;
    }
}