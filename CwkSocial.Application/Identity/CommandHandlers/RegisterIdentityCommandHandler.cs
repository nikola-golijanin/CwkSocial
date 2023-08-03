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
            await CheckIfIdentityUserDoesNotExist(result, request);
            if (request.IsError) return result;

            //creating transaction
            await using var transaction = _context.Database.BeginTransaction();

            var identityUser = await CreateIdentityUserAsync(result, request, transaction);
            if (result.IsError) return result;

            var userProfile = await CreateUserProfileAsync(result, request, transaction, identityUser);
            await transaction.CommitAsync();

            result.Payload = GetJwtString(identityUser, userProfile);
            return result;
        }
        catch (UserProfileNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e => result.AddError(ErrorCode.ValidationError,e));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }


    private async Task CheckIfIdentityUserDoesNotExist(OperationResult<string> result,
        RegisterIdentityCommand request)
    {
        var existingIdentity = await _userManager.FindByEmailAsync(request.Username);

        if (existingIdentity != null)
            result.AddError(ErrorCode.IdentityUserAlreadyExists,IdentityErrorMessages.IdentityUserAlreadyExists);
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

        if (!identityCreatedResult.Succeeded)
        {
            await transaction.RollbackAsync();

            foreach (var identityCreatedError in identityCreatedResult.Errors)
            {
                result.AddError(ErrorCode.IdentityCreationFailed,identityCreatedError.Description);
            }
        }

        return identity;
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

    private string GetJwtString(IdentityUser identityUser, UserProfile userProfile)
    {
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
            new Claim("IdentityId", identityUser.Id),
            new Claim("UserProfileId", userProfile.UserProfileId.ToString())
        });

        var token = _identityService.CreateSecurityToken(claimsIdentity);
        return _identityService.WriteToken(token);
    }
}