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
    public IActionResult getPosts([FromQuery] int limit, int offset)
    {
        
        var posts = _postService.getposts(limit, offset);
        return Ok(posts);
    }
    
    
    /*
     * sends a posts id to the PostService and returns a post
     */
    [HttpGet]
    [Route("/api/post/{id}")]
    public IActionResult getFullPost([FromRoute] int id)
    {
        var post = _postService.getpost(id);
        return Ok(post);
    }
    
    /*
     * sends a posts id to the Post service class
     */
    [HttpDelete]
    [Route("/api/deletepost")]
    public IActionResult deletePost([FromQuery] int id)
    {
            _postService.deletePost(id);
            return Ok();
    }
    
    /*
     * sends data from a PostDto to the Post service class
     */
    [HttpPut]
    [Route("/api/post/{id}")]
    public IActionResult UpdatePost(PostDto dto, [FromRoute] int id)
    {
            return Ok(_postService.UpdatePost(id, dto.text, dto.imgurl));
    }
    
    [HttpGet]
    [Route("/api/userprofilepost/{id}")]
    public IActionResult getUserPost([FromRoute] int id)
    {
        var posts = _postService.getProfileposts(10, 0, id);
        return Ok(posts);
    }

    [HttpGet]
    [Route("/api/getmoreprofileposts/{id}")]
    public IActionResult getMoreProfilePosts([FromRoute] int id, [FromQuery] int limit, int offset)
    {
        var posts = _postService.getProfileposts(limit, offset, id);
        return Ok(posts);
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
            if (image?.Length > 10 * 1024 * 1024) return StatusCode(StatusCodes.Status413PayloadTooLarge);
            string? imgUrl = url;
            if (image != null)
            {
                try
                {
                    using var imageTransform = new ImageTransform(image.OpenReadStream())
                        .Resize(900, 450)
                        .RemoveMetadata()
                        .Jpeg();
                    // "postimages" is the container name
                    imgUrl = _blobService.Save("postimages", imageTransform.ToStream(), imgUrl);
                }
                catch (Exception e)
                {
                    throw new Exception("failed to upload image", e);
                }
            }

            var res = new ResponseDto { MessageToClient = imgUrl };
            return Ok(res);
    }

    /*
     * sends the id of the current user, limit and offset to the PostService class
     */
    [HttpGet]
    [RequireAuthentication]
    [Route("/api/getfollowedposts")]
    public IActionResult getFollowedposts([FromQuery] int limit, int offset)
    {
        var sessionData = HttpContext.GetSessionData();
        if (sessionData == null) return BadRequest("User is not authenticated");
        var posts = _postService.getFollowedPosts(sessionData.UserId, limit, offset);
        return Ok(posts);
    }
    
}