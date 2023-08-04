using AutoMapper;
using CwkSocial.Api.Contracts.Common;
using CwkSocial.Api.Contracts.Posts.Requests;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Extensions;
using CwkSocial.Api.Filters;
using CwkSocial.Application.Posts.Commands;
using CwkSocial.Application.Posts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.BaseRoute)]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        var result = await _mediator.Send(query);
        var posts = _mapper.Map<List<PostResponse>>(result.Payload);
        return Ok(posts);
    }

    [HttpGet]
    [ValidateGuid("id")]
    [Route(ApiRoutes.Posts.IdRoute)]
    public async Task<IActionResult> GetPostById(string id)
    {
        var query = new GetPostByIdQuery() { PostId = Guid.Parse(id) };
        var result = await _mediator.Send(query);

        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        var post = _mapper.Map<PostResponse>(result.Payload);
        return Ok(post);
    }


    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreatePost([FromBody] PostCreate newPost)
    {
        var userProfileId = HttpContext.GetUserProfileId();
        
        var command = new CreatePostCommand
        {
            UserProfileId = userProfileId,
            TextContent = newPost.TextContent
        };

        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        var post = _mapper.Map<PostResponse>(result.Payload);

        return CreatedAtAction(nameof(GetPostById),
            new { id = post.PostId }, post);
    }

    [HttpPatch]
    [Route(ApiRoutes.Posts.IdRoute)]
    [ValidateModel]
    [ValidateGuid("id")]
    public async Task<IActionResult> UpdatePostText([FromRoute] string id, [FromBody] PostUpdate post)
    {
        var userProfileId = HttpContext.GetUserProfileId();

        var command = new UpdatePostTextCommand
        {
            UserProfileId = userProfileId,
            PostId = Guid.Parse(id),
            TextContent = post.TextContent
        };

        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.Posts.IdRoute)]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var userProfileId = HttpContext.GetUserProfileId();

        var command = new DeletePostCommand
            { 
                PostId = Guid.Parse(id),
                UserProfileId = userProfileId
            };
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        return NoContent();
    }

    [HttpGet]
    [Route(ApiRoutes.Posts.PostComments)]
    [ValidateGuid("postId")]
    public async Task<IActionResult> GetCommentsByPostId([FromRoute] string postId)
    {
        var query = new GetPostCommentsQuery { PostId = Guid.Parse(postId) };
        var result = await _mediator.Send(query);

        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        var comments = _mapper.Map<IEnumerable<PostCommentResponse>>(result.Payload);
        return Ok(comments);
    }

    [HttpPost]
    [Route(ApiRoutes.Posts.PostComments)]
    [ValidateGuid("postId")]
    [ValidateModel]
    public async Task<IActionResult> AddCommentToPost([FromRoute] string postId,
        [FromBody] PostCommentCreate newComment)
    {
        var isUserProfileIdValid = Guid.TryParse(newComment.UserProfileId, out var userProfileId);

        if (!isUserProfileIdValid)
        {
            var apiError = new ErrorResponse
            {
                StatusCode = 400,
                StatusPhrase = "Bad request",
                Timestamp = DateTime.Now,
                Errors = { "Provided User profile ID is not in valid Guid format" }
            };
            return BadRequest(apiError);
        }

        var command = new AddPostCommentCommand
        {
            PostId = Guid.Parse(postId),
            Text = newComment.Text,
            UserProfileId = userProfileId
        };

        var result = await _mediator.Send(command);
        if (result.IsError)
        {
            return HandleErroroResponse(result.Errors);
        }

        var comment = _mapper.Map<PostCommentResponse>(result.Payload);
        return Ok(comment);
    }

    [HttpPut]
    [Route(ApiRoutes.Posts.CommentById)]
    [ValidateGuid("postId", "commentId")]
    [ValidateModel]
    public async Task<IActionResult> UpdateCommentText([FromRoute] string postId, [FromRoute]string commentId,
        [FromBody] PostCommentUpdate updatedComment)
    {
        var command = new UpdatePostCommentCommand
        {
            UserProfileId = Guid.Parse(updatedComment.UserProfileId),
            PostId = Guid.Parse(postId),
            CommentId = Guid.Parse(commentId),
            UpdatedText = updatedComment.Text
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsError) return HandleErroroResponse(result.Errors);
            
        return NoContent();
    }
    
    [HttpGet]
    [Route(ApiRoutes.Posts.PostInteractions)]
    [ValidateGuid("postId")]
    public async Task<IActionResult> GetPostInteractions([FromRoute] string postId)
    {
    
        var query = new GetPostInteractions
        {
            PostId = Guid.Parse(postId),
        };

        var result = await _mediator.Send(query);

        if (result.IsError) return HandleErroroResponse(result.Errors);

        var mappedResult = _mapper.Map<IEnumerable<PostInteractionResponse>>(result.Payload);
        return Ok(mappedResult);
    }

    [HttpPost]
    [Route(ApiRoutes.Posts.PostInteractions)]
    [ValidateGuid("postId")]
    [ValidateModel]
    public async Task<IActionResult> AddPostInteraction([FromRoute] string postId, [FromBody] PostInteractionCreate interaction)
    {
        var postGuid = Guid.Parse(postId);
        var userProfileId = HttpContext.GetUserProfileId();

        var command = new AddInteraction
        {
            PostId = postGuid,
            UserProfileId = userProfileId,
            Type = interaction.Type
        };

        var result = await _mediator.Send(command, token);

        if (result.IsError) HandleErrorResponse(result.Errors);

        var mapped = _mapper.Map<PostInteractionResponse>(result.Payload);

        return Ok(mapped);
    }
}