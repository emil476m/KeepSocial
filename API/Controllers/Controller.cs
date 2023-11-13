using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{

    private readonly ILogger<Controller> _logger;
    private readonly JwtService _jwtService;

    public Controller(ILogger<Controller> logger, JwtService jwtService)
    {
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpGet("/IsEven{number}")]
    public bool isEven([FromRoute]int number)
    {
        return (number % 2) == 0;
    }
}
