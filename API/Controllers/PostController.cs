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
        
        
        var post = new Post
        {
            id = dto.id,
            author_id = HttpContext.GetSessionData()!.UserId,
            text = dto.text,
            img_url = dto.imgurl,
            created = timestamp,
            name = "",
        };
        if (post == null) return BadRequest();
        return Ok(_postService.createPost(post));
    }
}