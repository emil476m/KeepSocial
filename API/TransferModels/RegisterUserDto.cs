using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class RegisterUserDto
{
    [Required] public required string Name { get; set; }

    [Required] public required string Email { get; set; }

    [Required] [MinLength(8)] public required string Password { get; set; }

    public DateTime Userbirthday { get; set; }
}