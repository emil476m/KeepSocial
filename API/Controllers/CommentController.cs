using API.Filters;
using API.TransferModels;
using Infastructure;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CommentController: ControllerBase
{
    private readonly CommentService _commentService;

    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    [Route("/api/comment/")]
    [RequireAuthentication]
    public IActionResult createComment([FromBody] CommentDto dto, [FromQuery]int postId)
    {
        var comment = new Comment
        {
            post_id = postId,
            author_id = HttpContext.GetSessionData().UserId,
            text = dto.text,
            img_url = dto.imgurl
        };
        if (comment == null) return BadRequest("Failed to create comment");
        var commentdb = _commentService.createComment(comment);
        return Ok(commentdb);
    }
    
    [HttpGet]
    [Route("/api/getcomments")]
    public IActionResult getComments([FromQuery] int postId)
    {
        try
        {
            var posts = _commentService.getComments(10, 0, postId);
            return Ok(posts);   
        }
        catch
        {
            return BadRequest("failed to get comments please try again");
        }
    }

    [HttpGet]
    [Route("/api/getmorecomments")]
    public IActionResult getmoreComments([FromQuery] int limit, int offset, int postId)
    {
        try
        {
            var posts = _commentService.getComments(limit, offset, postId);
            return Ok(posts);
        }
        catch
        {
            return BadRequest("failed to get more posts please try again");
        }
    }
}