using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{

    private readonly ILogger<Controller> _logger;

    public Controller(ILogger<Controller> logger)
    {
        _logger = logger;
    }

    [HttpGet("/IsEven{number}")]
    public bool isEven([FromRoute]int number)
    {
        return (number % 2) == 0;
    }
}
