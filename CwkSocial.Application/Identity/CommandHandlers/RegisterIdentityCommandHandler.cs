using System.IdentityModel.Tokens.Jwt;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<string>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterIdentityCommandHandler(DataContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<OperationResult<string>> Handle(RegisterIdentityCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        try
        {
            var existingIdentity = await _userManager.FindByEmailAsync(request.Username);

            if (existingIdentity != null)
            {
                result.IsError = true;
                var error = new Error
                {
                    Code = ErrorCode.IdentityUserAlredyExists,
                    Message = "Provided email address already exists. Cannot register new user"
                };
                result.Errors.Add(error);
                return result;
            }

            var identity = new IdentityUser
            {
                Email = request.Username,
                UserName = request.Username
            };

            using var transaction = _context.Database.BeginTransaction();

            var createdIdentity = await _userManager.CreateAsync(identity, request.Password);
            if (!createdIdentity.Succeeded)
            {
                result.IsError = true;

                foreach (var identityError in createdIdentity.Errors)
                {
                    var error = new Error
                    {
                        Code = ErrorCode.IdentityCreationFailed,
                        Message = identityError.Description
                    };
                    result.Errors.Add(error);
                }

                return result;
            }

            var profileInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.Username,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            var profile = UserProfile.CreateUserProfile(identity.Id, profileInfo);

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            //creating transaction
            var tokenHandler = new JwtSecurityTokenHandler();
        }
        catch (UserProfileNotValidException ex)
        {
            result.IsError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                var error = new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = $"{ex.Message}"
                };
                result.Errors.Add(error);
            });
        }
        catch (Exception e)
        {
            var error = new Error
            {
                Code = ErrorCode.UnknownError,
                Message = $"{e.Message}"
            };
            result.IsError = true;
            result.Errors.Add(error);
        }

        return result;
    }
}