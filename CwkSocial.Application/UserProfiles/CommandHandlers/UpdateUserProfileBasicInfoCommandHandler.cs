﻿using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class
    UpdateUserProfileBasicInfoCommandHandler : IRequestHandler<UpdateUserProfileBasicInfoCommand,
        OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public UpdateUserProfileBasicInfoCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(UpdateUserProfileBasicInfoCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();
        try
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(profile => profile.UserProfileId == request.UserProfileId);

            if (userProfile is null)
            {
                result.AddError(ErrorCode.NotFound,
                    string.Format(UserProfilesErrorMessages.UserProfileNotFound, request.UserProfileId));
                return result;
            }

            var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            userProfile.UpdateBasicInfo(basicInfo);

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            result.Payload = userProfile;
        }
        catch (UserProfileNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e => result.AddError(ErrorCode.ValidationError, e));
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }
}