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

    public AccountController(AccountService accountService, JwtService jwtService, HttpClientService httpClientService)
    {
        _accountService = accountService;
        _jwtService = jwtService;
        _clientService = httpClientService;
        
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
    
    [HttpGet]
    [RequireAuthentication]
    [Route("/account/getAllUsers")]
    public IEnumerable<User> getTest()
    {
        return _accountService.getUserName();
    }
    
    [HttpPost]
    [Route("/api/account/login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _accountService.Authenticate(dto.email, dto.password);
        if (user == null) return Unauthorized("Email or password was wrong please try again");
        var token = _jwtService.IssueToken(SessionData.FromUser(user!));
        return Ok(new { token });
    }
    
    
    
    
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
    
}