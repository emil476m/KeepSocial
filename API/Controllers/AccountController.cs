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
    [Route("/api/account/register")]
    public ResponseDTO.ResponseDto Register([FromBody] RegisterUserDto dto)
    {
        var user = _accountService.CreateUser(dto.FullName, dto.Email, dto.Password, dto.userBirthday);
        return new ResponseDTO.ResponseDto
        {
            MessageToClient = "Successfully registered"
        };
    }
}