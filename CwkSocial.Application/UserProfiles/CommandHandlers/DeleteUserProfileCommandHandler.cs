using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public DeleteUserProfileCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(DeleteUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();
        try
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(profile => profile.UserProfileId == request.UserProfileId);

            if (userProfile is null)
            {
                result.IsError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No UserProfile with ID {request.UserProfileId}" };
                result.Errors.Add(error);
                return result;
            }

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();

            result.Payload = userProfile;
        }
        catch (Exception e)
        {
            var error = new Error { Code = ErrorCode.InternalServerError, Message = e.Message };
            result.IsError = true;
            result.Errors.Add(error);
        }

        return result;
    }
}