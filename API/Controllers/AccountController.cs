using System.ComponentModel.DataAnnotations;
using API.Filters;
using API.TransferModels;
using Infastructure;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;


[Route("/api/account/")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly JwtService _jwtService;
    private readonly HttpClientService _clientService;
    private readonly BlobService _blobService;

    public AccountController(AccountService accountService, BlobService blobService, JwtService jwtService,
        HttpClientService httpClientService)
    {
        _accountService = accountService;
        _jwtService = jwtService;
        _clientService = httpClientService;
        _blobService = blobService;

    }



    [HttpPost]
    [Route("/api/account/createuser")]
    public ResponseDto CreateUser([FromBody] RegisterUserDto dto)
    {
        var user = _accountService.CreateUser(dto.userDisplayName, dto.userEmail, dto.password, dto.userBirthday);
        return new ResponseDto
        {
            MessageToClient = "Successfully registered"
        };
    }
    
    /**
     * sends the login credentials from the Login dto to the AccountService class 
     */
    [HttpPost]
    [RateLimiter(5)]
    [Route("/api/account/login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _accountService.Authenticate(dto.email, dto.password);
        if (user == null) return Unauthorized("Email or password was wrong please try again");
        var token = _jwtService.IssueToken(SessionData.FromUser(user!));
        return Ok(new { token });
    }
    
    
    
    /*
     * gets the sight key for recapcha
     */
    [HttpGet]
    [Route("/api/skey")]
    public ResponseDto getSitekey()
    {
        var key = Environment.GetEnvironmentVariable("rekey");
        return new ResponseDto
        {
            ResponseData = new { key },
        };
    }

    /*
     * sends the current user's id to the AccountService class and returns that User object 
     */
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/whoami")]
    public User whoAmI()
    {
        int id = HttpContext.GetSessionData().UserId;
        return _accountService.whoAmI(id);
    }
    
    
    
    [RequireAuthentication]
    [HttpPost]
    [Route("/api/account/updateAccount")]
    public ResponseDto updateAccount([FromBody] UpdateBasicUserDataDto dto)
    { 
        bool success = _accountService.UpdateUser(HttpContext.GetSessionData().UserId, dto.updatedValue, dto.updatedValueName);
        if (success)
        {
            return new ResponseDto
            {
                MessageToClient = "Successfully updated"
            };
        }
        else
        {
            return new ResponseDto
            {
                MessageToClient = "Failed to update"
            };
        }
    }

    /*
     * sends a validation request and returns true or false depending on the result
     */
    [HttpPost]
    [Route("/api/ishuman")]
    public async Task<ResponseDto> ishuman([FromBody] RecaptchaTokenDTO dto)
    {
        var ishuman = await _clientService.verifyHuman(dto.token);
        return new ResponseDto
        {
            ResponseData = new {ishuman}
        };
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/freinds")]
    public IEnumerable<User> GetFreinds(int pageNumber)
    {
        int userId = HttpContext.GetSessionData().UserId!;
        return _accountService.getFriends(userId, pageNumber);
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/account/validationGeneration")]
    public ResponseDto ValidationGeneration()
    {
        bool success = _accountService.SendEmailValidation(HttpContext.GetSessionData().UserId, null);
        if (success)
        {
            return new ResponseDto
            {
                MessageToClient = "Successfully Sent Code"
            };
        }
        else
        {
            return new ResponseDto
            {
                MessageToClient = "Failed to update"
            };
        }
    }
    
    [RequireAuthentication]
    [HttpPost]
    [Route("/api/account/followUser")]
    public ResponseDto FollowUser([FromBody] int followedId)
    { 
        bool success = _accountService.FollowUser(HttpContext.GetSessionData().UserId, followedId);
        if (success)
        {
            return new ResponseDto
            {
                MessageToClient = "Successfully followed"
            };
        }
        else
        {
            return new ResponseDto
            {
                MessageToClient = "Failed to follow"
            };
        }
    }
    
    [RequireAuthentication]
    [HttpDelete]
    [Route("/api/account/unFollowUser/{followedId}")]
    public ResponseDto UnFollowUser([FromRoute] int followedId)
    { 
        bool success = _accountService.UnFollowUser(HttpContext.GetSessionData().UserId, followedId);
        if (success)
        {
            return new ResponseDto
            {
                MessageToClient = "Successfully unfollowed"
            };
        }
        else
        {
            return new ResponseDto
            {
                MessageToClient = "Failed to unfollow"
            };
        }
    }
    
    [RequireAuthentication]
    [HttpPost]
    [Route("/api/account/checkIfFollowing")]
    public ReturnBoolDto CheckIfFollowing([FromBody] int followedId)
    { 
        bool isFollowing = _accountService.CheckIfFollowing(HttpContext.GetSessionData().UserId, followedId);
        return new ReturnBoolDto
        {
            isTrue = isFollowing
        };
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/getProfile/{profileName}")]
    public Profile getProfile([FromRoute] string profileName)
    {
        
        int id = HttpContext.GetSessionData().UserId;
        return _accountService.getProfile(profileName, id);
    }

    [RequireAuthentication]
    [HttpPut]
    [Route("/api/account/updateAvatar")]
    public IActionResult Update([FromForm] IFormFile? avatar)
    {
        if (avatar?.Length > 10 * 1024 * 1024) return StatusCode(StatusCodes.Status413PayloadTooLarge);
        var session = HttpContext.GetSessionData()!;
        string? avatarUrl = null;
        if (avatar != null)
        {
            //returns user, and then we only takes the avatar url string
            avatarUrl = this._accountService.whoAmI(session.UserId)?.avatarUrl;
            // We need a stream of bytes (image data)
            using var AvatarTransform = new ImageTransform(avatar.OpenReadStream())
                .Resize(200,200)
                .FixOrientation()
                .RemoveMetadata()
                .Jpeg();
            // "avatar" is the container name
            avatarUrl = _blobService.Save("avatar", AvatarTransform.ToStream(),avatarUrl);
        }

        _accountService.UpdateAvatar(session, avatarUrl);
        return Ok();
    }


    /**
     * returns a list of FriendRequest that have been send to a friend request
     */
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/account/GetFriendRequests")]
    public IEnumerable<FriendRequestModel> GetRequests(int pageNumber)
    {
        int userId = HttpContext.GetSessionData().UserId!;

        return _accountService.GetFriendRequest(userId, pageNumber);
    }
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/account/FriendRequestsResponse")]
    public ResponseDto RequestsResponse(RequestUpdateDto response)
    {
        int userId = HttpContext.GetSessionData().UserId!;

        string messageToClient = _accountService.handleFriendRequest(response.Response, response.RequestId, response.RequesterId, userId);
        
        return new ResponseDto
        {
            MessageToClient = messageToClient
        };
    }
    
    [HttpGet]
    [Route("/api/Search/{searchTerm}")]
    public IActionResult profileSearch([FromRoute] string searchTerm, [FromQuery] int limit, [FromQuery]int offset)
    {
        try
        {
            var searchResult = _accountService.profileSearch(limit, offset,searchTerm);
            return Ok(searchResult);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            return BadRequest("failed to get search results please try again");
        }
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/account/haveSendFriendRequest{requestingId}")]
    public IActionResult HaveSendFriendRequest([FromRoute]int requestingId)
    {
        int userid = HttpContext.GetSessionData().UserId;

        return Ok(_accountService.HaveSendFriendRequest(userid, requestingId));
        
    }
    
    [RequireAuthentication]
    [HttpPost]
    [Route("/api/account/SendFriendRequest{requestingId}")]
    public IActionResult SendFriendRequest([FromRoute]int requestingId)
    {
        int userid = HttpContext.GetSessionData().UserId;
        
        return Ok(_accountService.SendFriendRequest(userid, requestingId));

    }
    
    /*
     * sends id, offset and limit to the account service class and returns a list of SimpleUser objects
     */
    [HttpGet]
    [RequireAuthentication]
    [Route("/api/account/getfollowers")]
    public IActionResult getfollowers([FromQuery] int id,int offset, int limit)
    {
        try
        {
            return Ok(_accountService.getFollowers(id, offset,limit));
        }
        catch (Exception e)
        {
            return NoContent();
        }
    }
    
    /*
     * sends id, offset and limit to the account service class and returns a list of SimpleUser objects
     */
    [HttpGet]
    [RequireAuthentication]
    [Route("/api/account/getfollowing")]
    public IActionResult getfollowing([FromQuery] int id,int offset, int limit)
    {
        try
        {
            return Ok(_accountService.getFollowing(id, offset,limit));
        }
        catch (Exception e)
        {
            return NoContent();
        }
    }

}