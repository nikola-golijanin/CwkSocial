using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;

namespace CwkSocial.Application.UserProfiles.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<UserProfile>>
{
    private readonly DataContext _context;

    public CreateUserCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserProfile>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<UserProfile>();

        try
        {
            var basicInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.EmailAddress,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            var userProfile = UserProfile.CreateUserProfile(Guid.NewGuid().ToString(), basicInfo);
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            result.Payload = userProfile;
        }
        catch (UserProfileNotValidException ex)
        {
            result.isError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                var error = new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = $"{ex.Message}",
                };
                result.Errors.Add(error);
            });
        }
        catch (Exception ex)
        {
            result.isError = true;
            var error = new Error { Code = ErrorCode.UnknownError, Message = ex.Message };
            result.Errors.Add(error);
        }

        return result;
    }
}