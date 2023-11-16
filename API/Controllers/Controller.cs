
using API.TransferModels;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{

    private readonly ILogger<Controller> _logger;
    private readonly JwtService _jwtService;
    private readonly HttpClientService _clientService;

    public Controller(ILogger<Controller> logger, JwtService jwtService, HttpClientService clientService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _clientService = clientService;
    }

    [HttpGet("/IsEven{number}")]
    public bool isEven([FromRoute]int number)
    {
        return (number % 2) == 0;
    }
    
    [HttpPost]
    [Route("/api/account/login")]
    public ResponseDto Login([FromBody] LoginDto dto)
    {
      //  var user = _service.Authenticate(dto.Email, dto.Password);
       // var token = _jwtService.IssueToken(SessionData.FromUser(user!));
        return new ResponseDto
        {
            MessageToClient = "Login Successfull",
        //    ResponseData = new { token },
        };
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
