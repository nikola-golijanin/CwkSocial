using AutoMapper;
using CwkSocial.Api.Contracts.UserProfile.Requests;
using CwkSocial.Api.Contracts.UserProfile.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserProfilesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UserProfilesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    //TODO provide cancelation token everywhere
    [HttpGet]
    public async Task<IActionResult> GetAllProfiles()
    {
        var query = new GetAllUserProfilesQuery();
        var result = await _mediator.Send(query);
        var profiles = _mapper.Map<List<UserProfileResponse>>(result.Payload);
        return Ok(profiles);
    }

    [HttpGet]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetUserProfileById(string id)
    {
        var query = new GetUserProfileByIdQuery { UserProfileId = Guid.Parse(id) };
        var result = await _mediator.Send(query);

        if (result.IsError)
        {
            return HandleErrorResponse(result.Errors);
        }

        var userProfile = _mapper.Map<UserProfileResponse>(result.Payload);
        return Ok(userProfile);
    }

    [HttpPatch]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateModel]
    [ValidateGuid("id")]
    public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserProfileCreateOrUpdate profile)
    {
        var command = _mapper.Map<UpdateUserProfileBasicInfoCommand>(profile);
        command.UserProfileId = Guid.Parse(id);
        var result = await _mediator.Send(command);

        return result.IsError
            ? HandleErrorResponse(result.Errors)
            : NoContent();
    }
}