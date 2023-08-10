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
using CwkSocial.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace CwkSocial.Application.Identity.CommandHandlers;

public class
    RegisterIdentityCommandHandler : IRequestHandler<RegisterIdentityCommand, OperationResult<IdentityUserProfileDto>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityService _identityService;
    private readonly IMapper _mapper;
    private OperationResult<IdentityUserProfileDto> _result = new();

    public RegisterIdentityCommandHandler(DataContext context, UserManager<IdentityUser> userManager,
        IdentityService identityService, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _identityService = identityService;
        _mapper = mapper;
    }


    public async Task<OperationResult<IdentityUserProfileDto>> Handle(RegisterIdentityCommand request,
        CancellationToken cancellationToken)
    {
        var result = new OperationResult<string>();
        try
        {
            await CheckIfIdentityUserDoesNotExist(request);
            if (_result.IsError) return _result;

            //creating transaction
            await using var transaction = _context.Database.BeginTransaction();

            var identityUser = await CreateIdentityUserAsync(request, transaction);
            if (_result.IsError) return _result;

            var userProfile = await CreateUserProfileAsync(request, transaction, identityUser);
            await transaction.CommitAsync();

            _result.Payload = _mapper.Map<IdentityUserProfileDto>(userProfile);
            _result.Payload.UserName = identityUser.UserName;
            _result.Payload.Token = GetJwtString(identityUser, userProfile);
            return _result;
        }
        catch (UserProfileNotValidException ex)
        {
            ex.ValidationErrors.ForEach(e => result.AddError(ErrorCode.ValidationError, e));
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return _result;
    }


    private async Task CheckIfIdentityUserDoesNotExist(
        RegisterIdentityCommand request)
    {
        var existingIdentity = await _userManager.FindByEmailAsync(request.Username);

        if (existingIdentity != null)
            _result.AddError(ErrorCode.IdentityUserAlreadyExists, IdentityErrorMessages.IdentityUserAlreadyExists);
    }

    private async Task<IdentityUser> CreateIdentityUserAsync(
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
                _result.AddError(ErrorCode.IdentityCreationFailed, identityCreatedError.Description);
            }
        }

        return identity;
    }

    private async Task<UserProfile> CreateUserProfileAsync(RegisterIdentityCommand request,
        IDbContextTransaction transaction, IdentityUser identity)
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
        catch (Exception ex)
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