using System.Net.Mime;
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
    private BlobService _blobService;

    public CommentController(CommentService commentService, BlobService blobService)
    {
        _commentService = commentService;
        _blobService = blobService;
    }

    
    /*
     * sends data from a CommentDto to the CommentService class along with the id to the post the comment belongs to and returns that comment
     */
    [HttpPost]
    [Route("/api/comment/")]
    [RequireAuthentication]
    public IActionResult createComment([FromBody] CommentDto dto, [FromQuery]int postId)
    {
        var comment = new Comment
        {
            postId = postId,
            authorId = HttpContext.GetSessionData().UserId,
            text = dto.text,
            imgUrl = dto.imgurl
        };
        if (comment == null) return BadRequest("Failed to create comment");
        var commentdb = _commentService.createComment(comment);
        return Ok(commentdb);
    }
    
    /*
     * gets an initial amount of comments 
     */
    [HttpGet]
    [Route("/api/getcomments")]
    public IActionResult getComments([FromQuery] int postId, int limit, int offset)
    {
        try
        {
            var posts = _commentService.getComments(limit, offset, postId);
            return Ok(posts);   
        }
        catch(Exception e)
        {
            return BadRequest("failed to get comments please try again");
        }
    }

    /*
     * sends the id to a comment to the CommentService class
     */
    [HttpDelete]
    [Route("/api/deletecomment")]
    public IActionResult deletecomment([FromQuery] int id)
    {
        try
        {
            _commentService.deleteComment(id);
            return Ok();
        }
        catch(Exception e)
       
        { 
            return BadRequest("Failed to delete comment try again");
        }
    }
    
    /*
     * sends new data from a CommentDto to the CommentService class
     */
    [HttpPut]
    [Route("/api/comment/{id}")]
    public Comment UpdateComment(CommentDto dto, [FromRoute] int id)
    {
        return _commentService.UpdateComment(id, dto.text, dto.imgurl);
    }

    /*
     * uploades the comment image to blobstorage and returns the image's url
     */
    [HttpPost]
    [RequireAuthentication]
    [Route("/api/commentimg")]
    public IActionResult uploadImg([FromForm] IFormFile? image, [FromQuery] string? url)
    {
        if (url.Equals("undefined"))
        {
            url = null;
        }

        try
        {
            if (image?.Length > 10 * 1024 * 1024) return StatusCode(StatusCodes.Status413PayloadTooLarge);
            string? imgUrl = url;
            if (image != null)
            {
             
                using var imageTransform = new ImageTransform(image.OpenReadStream())
                    .Resize(900, 450)
                    .RemoveMetadata()
                    .Jpeg();
                // "postimages" is the container name
                imgUrl = _blobService.Save("commentimages", imageTransform.ToStream(), imgUrl);
                
            }

            var res = new ResponseDto { MessageToClient = imgUrl };
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest("Failed to upload image");
        }
    }
}