using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<string>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;

    public LoginCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
        IdentityService identityService)
    {
        _context = context;
        _userManager = userManager;
        _identityService = identityService;
    }

    public async Task<OperationResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        try
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Username);

            if (identityUser is null)
            {
                result.IsError = true;
                var error = new Error
                {
                    Code = ErrorCode.IdewntityUserDoesNotExist,
                    Message = $"Unable to find user with that email. Login Failed."
                };
                result.Errors.Add(error);
                return result;
            }

            var validPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

            if (!validPassword)
            {
                result.IsError = true;
                var error = new Error
                {
                    Code = ErrorCode.IncorrectPassword,
                    Message = $"Wrong password. Login Failed"
                };
                result.Errors.Add(error);
                return result;
            }

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim("IdentityId", identityUser.Id),
                new Claim("UserProfileId", userProfile.UserProfileId.ToString())
            });

            var token = _identityService.CreateSecurityToken(claimsIdentity);
            result.Payload = _identityService.WriteToken(token);
            return result;
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