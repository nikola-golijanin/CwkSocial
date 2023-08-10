using AutoMapper;
using CwkSocial.Application.Identity.Dtos;
using CwkSocial.Application.Identity.Queries;
using CwkSocial.Application.Models;
using CwkSocial.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Application.Identity.QueryHandlers;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUser, OperationResult<IdentityUserProfileDto>>
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMapper _mapper;
    private OperationResult<IdentityUserProfileDto> _result = new();

    public GetCurrentUserHandler(DataContext context, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<OperationResult<IdentityUserProfileDto>> Handle(GetCurrentUser request, 
        CancellationToken cancellationToken)
    {
        var identity = await _userManager.FindByIdAsync(request.IdentityId.ToString());

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserProfileId == request.UserProfileId, cancellationToken);

        _result.Payload = _mapper.Map<IdentityUserProfileDto>(profile);
        _result.Payload.UserName = identity.UserName;
        return _result;
    }
}