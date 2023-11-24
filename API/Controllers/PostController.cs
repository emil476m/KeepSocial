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
        
        var timestamp = DateTimeOffset.UtcNow;
        try
        {
            Console.WriteLine(HttpContext.GetSessionData()!.UserId);
        }
        catch
        {
            throw new Exception("could not get user id");
        }
        
        var post = new Post
        {
            id = dto.id,
            authorId = HttpContext.GetSessionData()!.UserId,
            text = dto.text,
            imgurl = dto.imgurl,
            created = timestamp,
        };
        if (post == null) return BadRequest();
        return Ok(_postService.createPost(post));
    }
}