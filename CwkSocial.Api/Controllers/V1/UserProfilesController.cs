﻿using AutoMapper;
using CwkSocial.Api.Contracts.UserProfile.Requests;
using CwkSocial.Api.Contracts.UserProfile.Responses;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
public class UserProfilesController : Controller
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
        var profiles = _mapper.Map<List<UserProfileResponse>>(response);
        return Ok(profiles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserProfile([FromBody] UserProfileCreateOrUpdate profile)
    {
        var command = _mapper.Map<CreateUserCommand>(profile);
        var response = await _mediator.Send(command);
        var userProfile = _mapper.Map<UserProfileResponse>(response);
        return CreatedAtAction(nameof(GetUserProfileById), new { id = response.UserProfileId }, userProfile);
    }

    [HttpGet]
    [Route(ApiRoutes.UserProfiles.GetById)]
    public async Task<IActionResult> GetUserProfileById(string id)
    {
        var query = new GetUserProfileByIdQuery { UserProfileId = Guid.Parse(id) };
        var response = await _mediator.Send(query);
        var userProfile = _mapper.Map<UserProfileResponse>(response);
        return Ok(userProfile);
    }

    [HttpPatch]
    [Route(ApiRoutes.UserProfiles.GetById)]
    public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserProfileCreateOrUpdate profile)
    {
        var command = _mapper.Map<UpdateUserProfileBasicInfoCommand>(profile);
        command.UserProfileId = Guid.Parse(id);
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.UserProfiles.GetById)]
    public async Task<IActionResult> DeletUserProfile(string id)
    {
        var command = new DeleteUserProfileCommand { UserProfileId = Guid.Parse(id) };
        await _mediator.Send(command);
        
        return NoContent();
    }
}