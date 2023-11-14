using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class RegisterUserDto
{
    [Required] public required string FullName { get; set; }

    [Required] public required string Email { get; set; }

    [Required] [MinLength(8)] public required string Password { get; set; }

    public DateOnly userBirthday { get; set; }
}