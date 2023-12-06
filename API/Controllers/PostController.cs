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
    private BlobService _blobService;

    public PostController(PostService postService, BlobService blobService)
    {
        _postService = postService;
        _blobService = blobService;
    }

    /*
     * sends data from a postDto to the Post service class and returns the post once it has been created 
     */
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

    /*
     * returns an initial amount of post 
     */
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
    
    /*
     * returns more posts to the user depending on the variables limit and off set
     */
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
    
    /*
     * sends a posts id to the PostService and returns a post
     */
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
    
    /*
     * sends a posts id to the Post service class
     */
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
    
    /*
     * sends data from a PostDto to the Post service class
     */
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
    
    /*
     * uploads an image to blobstorage and returns the image's url 
     */
    [HttpPost]
    [RequireAuthentication]
    [Route("/api/postimg")]
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
                    imgUrl = _blobService.Save("postimages", imageTransform.ToStream(), imgUrl);
                
            }

            var res = new ResponseDto { MessageToClient = imgUrl };
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest("Failed to upload image" + e);
        }
    }
}