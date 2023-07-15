using AutoMapper;
using CwkSocial.Api.Contracts.UserProfile.Requests;
using CwkSocial.Api.Contracts.UserProfile.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
public class UserProfilesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UserProfilesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProfiles()
    {
        var query = new GetAllUserProfilesQuery();
        var result = await _mediator.Send(query);
        var profiles = _mapper.Map<List<UserProfileResponse>>(result.Payload);
        return Ok(profiles);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateOrUpdate profile)
    {
        var command = _mapper.Map<CreateUserCommand>(profile);
        var result = await _mediator.Send(command);
        
        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }
        
        var userProfile = _mapper.Map<UserProfileResponse>(result.Payload);

        return CreatedAtAction(nameof(GetUserProfileById),
            new { id = userProfile.UserProfileId }, userProfile);
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
            return HandleErroroResponse(result.Errors);
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
            ? HandleErroroResponse(result.Errors)
            : NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteUserProfile(string id)
    {
        var command = new DeleteUserProfileCommand { UserProfileId = Guid.Parse(id) };
        var result = await _mediator.Send(command);

        return result.IsError
            ? HandleErroroResponse(result.Errors)
            : NoContent();
    }
}