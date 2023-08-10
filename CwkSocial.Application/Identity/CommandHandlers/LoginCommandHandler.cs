using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Models;
using CwkSocial.Application.Services;
using CwkSocial.DataAccess;
using CwkSocial.Domain.Aggregate.UserProfileAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<IdentityUserProfileDto>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;
    private readonly IMapper _mapper;
    private OperationResult<IdentityUserProfileDto> _result = new();

    public LoginCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
        IdentityService identityService,IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<OperationResult<IdentityUserProfileDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var identityUser = await ValidateAndGetIdentityUserAsync(request);

            if (_result.IsError) return _result;

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);

            _result.Payload = _mapper.Map<IdentityUserProfileDto>(userProfile);
            _result.Payload.UserName = identityUser.UserName;
            _result.Payload.Token = GetJwtString(identityUser, userProfile);
            return _result;
        }
        catch (Exception ex)
        {
            _result.AddUnknownError(ex.Message);
        }

        return _result;
    }

    private async Task<IdentityUser> ValidateAndGetIdentityUserAsync(LoginCommand request)
    {
        var identityUser = await _userManager.FindByEmailAsync(request.Username);

        if (identityUser is null)
            _result.AddError(ErrorCode.IdentityUserDoesNotExist, IdentityErrorMessages.NonExistentIdentityUser);

        var validPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

        if (!validPassword)
            _result.AddError(ErrorCode.IncorrectPassword, IdentityErrorMessages.IncorrectPassword);

        return identityUser;
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