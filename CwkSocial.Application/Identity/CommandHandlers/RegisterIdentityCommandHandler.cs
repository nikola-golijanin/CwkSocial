using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<string>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;


    public RegisterIdentityCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
        IdentityService identityService)
    {
        _context = context;
        _userManager = userManager;
        _identityService = identityService;
    }


    public async Task<OperationResult<string>> Handle(RegisterIdentityCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        try
        {
            var identityUserDoesNotExist = await CheckIfIdentityUserDoesNotExist(result, request);
            if (!identityUserDoesNotExist) return result;

            //creating transaction
            await using var transaction = _context.Database.BeginTransaction();

            var identity = await CreateIdentityUserAsync(result, request, transaction);
            if (identity is null) return result;

            var userProfile = await CreateUserProfileAsync(result, request, transaction, identity);
            await transaction.CommitAsync();

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identity.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identity.Email),
                new Claim("IdentityId", identity.Id),
                new Claim("UserProfileId", userProfile.UserProfileId.ToString())
            });

            var token = _identityService.CreateSecurityToken(claimsIdentity);
            result.Payload = _identityService.WriteToken(token);
            return result;
        }
        catch (UserProfileNotValidException ex)
        {
            result.IsError = true;
            ex.ValidationErrors.ForEach(e =>
            {
                var error = new Error
                {
                    Code = ErrorCode.ValidationError,
                    Message = $"{e}"
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


    private async Task<bool> CheckIfIdentityUserDoesNotExist(OperationResult<string> result,
        RegisterIdentityCommand request)
    {
        var existingIdentity = await _userManager.FindByEmailAsync(request.Username);

        if (existingIdentity != null)
        {
            result.IsError = true;
            var error = new Error
            {
                Code = ErrorCode.IdentityUserAlredyExists,
                Message = $"Provided email address already exists. Cannot register new user"
            };
            result.Errors.Add(error);
            return false;
        }

        return true;
    }

    private async Task<IdentityUser> CreateIdentityUserAsync(OperationResult<string> result,
        RegisterIdentityCommand request, IDbContextTransaction transaction)
    {
        var identity = new IdentityUser
        {
            Email = request.Username,
            UserName = request.Username
        };

        var identityCreatedResult = await _userManager.CreateAsync(identity, request.Password);

        if (identityCreatedResult.Succeeded)
        {
            return identity;
        }

        await transaction.RollbackAsync();
        result.IsError = true;

        foreach (var identityCreatedError in identityCreatedResult.Errors)
        {
            var error = new Error
            {
                Code = ErrorCode.IdentityCreationFailed,
                Message = identityCreatedError.Description
            };
            result.Errors.Add(error);
        }

        return null;
    }

    private async Task<UserProfile> CreateUserProfileAsync(OperationResult<string> result,
        RegisterIdentityCommand request, IDbContextTransaction transaction, IdentityUser identity)
    {
        try
        {
            var profileInfo = BasicInfo.CreateBasicInfo(request.FirstName, request.LastName, request.Username,
                request.Phone, request.DateOfBirth, request.CurrentCity);

            var userProfile = UserProfile.CreateUserProfile(identity.Id, profileInfo);
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}