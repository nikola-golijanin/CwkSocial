using AutoMapper;
using CwkSocial.Api.Contracts.Identity;
using CwkSocial.Api.Extensions;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Identity.Commands;
using CwkSocial.Application.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
public class IdentityController : BaseController
{
    private readonly IMediator _mediator;
    public readonly IMapper _mapper;

    public IdentityController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Route(ApiRoutes.Identity.Registration)]
    [ValidateModel]
    public async Task<IActionResult> Register(UserRegistration registration)
    {
        var command = _mapper.Map<RegisterIdentityCommand>(registration);
        var result = await _mediator.Send(command);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok(_mapper.Map<IdentityUserProfile>(result.Payload));
    }

    [HttpPost]
    [Route(ApiRoutes.Identity.Login)]
    [ValidateModel]
    public async Task<IActionResult> Login(Login login)
    {
        var command = _mapper.Map<LoginCommand>(login);
        var result = await _mediator.Send(command);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok(_mapper.Map<IdentityUserProfile>(result.Payload));
    }

    [HttpDelete]
    [Route(ApiRoutes.Identity.IdentityById)]
    [ValidateGuid("identityUserId")]
    [Authorize( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteAccount([FromRoute] string identityUserId)
    {
        var identityUserGuid = Guid.Parse(identityUserId);
        var requestorGuid = HttpContext.GetIdentityId();
        var command = new RemoveAccount
        {
            IdentityUserId = identityUserGuid,
            RequestorGuid = requestorGuid
        };
        var result = await _mediator.Send(command);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return NoContent();
    }
    
    [HttpGet]
    [Route(ApiRoutes.Identity.CurrentUser)]
    [Authorize( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CurrentUser(CancellationToken token)
    {
        var userProfileId = HttpContext.GetUserProfileId();
        var identityId = HttpContext.GetIdentityId();

        var query = new GetCurrentUser { UserProfileId = userProfileId, IdentityId = identityId};
        var result = await _mediator.Send(query);

        if (result.IsError) return HandleErrorResponse(result.Errors);

        return Ok(_mapper.Map<IdentityUserProfile>(result.Payload));
    }
}