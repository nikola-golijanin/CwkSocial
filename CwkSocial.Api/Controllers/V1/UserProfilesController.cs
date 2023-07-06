﻿using AutoMapper;
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
        var response = await _mediator.Send(query);
        var profiles = _mapper.Map<List<UserProfileResponse>>(response.Payload);
        return Ok(profiles);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateOrUpdate profile)
    {
        var command = _mapper.Map<CreateUserCommand>(profile);
        var response = await _mediator.Send(command);
        
        if (response.isError)
        {
            return HandleErroroResponse(response.Errors);
        }
        
        var userProfile = _mapper.Map<UserProfileResponse>(response.Payload);

        return CreatedAtAction(nameof(GetUserProfileById),
            new { id = userProfile.UserProfileId }, userProfile);
    }

    [HttpGet]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetUserProfileById(string id)
    {
        var query = new GetUserProfileByIdQuery { UserProfileId = Guid.Parse(id) };
        var response = await _mediator.Send(query);

        if (response.isError)
        {
            return HandleErroroResponse(response.Errors);
        }

        var userProfile = _mapper.Map<UserProfileResponse>(response.Payload);
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
        var response = await _mediator.Send(command);

        return response.isError
            ? HandleErroroResponse(response.Errors)
            : NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.UserProfiles.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteUserProfile(string id)
    {
        var command = new DeleteUserProfileCommand { UserProfileId = Guid.Parse(id) };
        var response = await _mediator.Send(command);

        return response.isError
            ? HandleErroroResponse(response.Errors)
            : NoContent();
    }
}