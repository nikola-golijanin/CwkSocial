using AutoMapper;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
public class PostsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PostsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var query = new GetAllPostsQuery();
        var response = await _mediator.Send(query);
        var posts = _mapper.Map<List<PostResponse>>(response.Payload);
        return Ok(response.Payload);
    }

    [HttpGet]
    [Route(ApiRoutes.Posts.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetById(string id)
    {
        var query = new GetPostByIdQuery() { PostId = Guid.Parse(id) };
        var result = await _mediator.Send(query);

        if (result.isError)
        {
            return HandleErroroResponse(result.Errors);
        }

        var post = _mapper.Map<PostResponse>(result.Payload);
        return Ok(post);
    }
}