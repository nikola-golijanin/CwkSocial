using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles;
using CwkSocial.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace CwkSocial.Application.Identity.Handlers;

public class RemoveAccountHandler : IRequestHandler<RemoveAccount, OperationResult<bool>>
{
    private readonly DataContext _context;

    public RemoveAccountHandler(DataContext context)
    {
        _context = context;
    }
    public async Task<OperationResult<bool>> Handle(RemoveAccount request, 
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        try
        {
            var identityUser = await _context.Users
                .FirstOrDefaultAsync(iu => iu.Id == request.IdentityUserId.ToString());

            if (identityUser == null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, 
                    IdentityErrorMessages.NonExistentIdentityUser);
                return result;
            }

            var userProfile = await _ctx.UserProfiles
                .FirstOrDefaultAsync(up => up.IdentityId == request.IdentityUserId.ToString());

            if (userProfile == null)
            {
                result.AddError(ErrorCode.NotFound, UserProfilesErrorMessages.UserProfileNotFound);
                return result;
            }

            if (identityUser.Id != request.RequestorGuid.ToString())
            {
                result.AddError(ErrorCode.UnauthorizedAccountRemoval, 
                    IdentityErrorMessages.UnauthorizedAccountRemoval);

                return result;
            }

            _context.UserProfiles.Remove(userProfile);
            _context.Users.Remove(identityUser);
            await _context.SaveChangesAsync(cancellationToken);

            result.Payload = true;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}