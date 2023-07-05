using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
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
                result.isError = true;
                var error = new Error
                    { Code = ErrorCode.NotFound, Message = $"No UserProfile with ID {request.UserProfileId}" };
                result.Errors.Add(error);
                return result;
            }

            var basicInfo = BasicInfo.TryCreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            userProfile.UpdateBasicInfo(basicInfo);

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            result.Payload = userProfile;
            return result;
        }
        catch (Exception e)
        {
            var error = new Error { Code = ErrorCode.InternalServerError, Message = e.Message };
            result.isError = true;
            result.Errors.Add(error);
        }

        return result;
    }
}