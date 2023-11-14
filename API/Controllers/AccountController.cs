using API.TransferModels;
using Infastructure;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }


    
    [HttpPost]
    [Route("/api/account/createuser")]
    public ResponseDto CreateUser([FromBody] RegisterUserDto dto)
    {
        var user = _accountService.CreateUser(dto.Name, dto.Email, dto.Password, dto.Userbirthday);
        return new ResponseDto
        {
            MessageToClient = "Successfully registered"
        };
    }
}