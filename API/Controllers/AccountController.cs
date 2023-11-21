using System.ComponentModel.DataAnnotations;
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
}