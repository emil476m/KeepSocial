using System.ComponentModel.DataAnnotations;
using API.Filters;
using API.TransferModels;
using Infastructure;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/post")]
    public IActionResult CreatePost([FromBody] PostDto dto)
    {
        var post = new Post
        {
            id = dto.id,
            authorId = HttpContext.GetSessionData()!.UserId,
            text = dto.text,
            imgUrl = dto.imgurl,
            authorName = "",
        };
        if (post == null) return BadRequest();
        return Ok(_postService.createPost(post));
    }

    [HttpGet]
    [Route("/api/getposts")]
    public IActionResult getPosts()
    {
        try
        {
            var posts = _postService.getposts(10, 0);
            return Ok(posts);
        }
        catch(Exception e)
        {
            return BadRequest("failed to get posts please try again");
        }
    }

    [HttpGet]
    [Route("/api/getmoreposts")]
    public IActionResult getmoreposts([FromQuery] int limit, int offset)
    {
        try
        {
            var posts = _postService.getposts(limit, offset);
            return Ok(posts);
        }
        catch(Exception e)
        {
            return BadRequest("failed to get more posts please try again");
        }
    }

    [HttpGet]
    [Route("/api/post/{id}")]
    public IActionResult getFullPost([FromRoute] int id)
    {
        try
        {
            var post = _postService.getpost(id);
            return Ok(post);
        }
        catch(Exception e)
        {
            return BadRequest("Failed to get the post you wanted please try again");
        }
    }
    
    [HttpDelete]
    [Route("/api/deletepost")]
    public IActionResult deletePost([FromQuery] int id)
    {
        try
        {
            _postService.deletePost(id);
            return Ok();
        }
        catch(Exception e)
       
        { 
            return BadRequest("Failed to delete comment try again");
        }
    }
    
    [HttpPut]
    [Route("/api/post/{id}")]
    public IActionResult UpdatePost(PostDto dto, [FromRoute] int id)
    {
        try
        {
            return Ok(_postService.UpdatePost(id, dto.text, dto.imgurl));
        }
        catch (Exception e)
        {
            return BadRequest("Failed to update post");
        }
    }
    
    [HttpGet]
    [Route("/api/userprofilepost/{id}")]
    public IActionResult getAllUserPost([FromRoute] int id)
    {
        try
        {
            var posts = _postService.getMoreProfileposts(10, 0, id);
            return Ok(posts);
        }
        catch(Exception e)
        {
            return BadRequest("Failed to get the posts for the user");
        }
    }

    [HttpGet]
    [Route("/api/getmoreprofileposts/{id}")]
    public IActionResult getMoreProfilePosts([FromRoute] int id, [FromQuery] int limit, int offset)

    {
        try
        {
            var posts = _postService.getMoreProfileposts(limit, offset, id);
            return Ok(posts);
        }
        catch (Exception e)
        {
            return BadRequest("failed to get posts please try again");
        }
    }
}