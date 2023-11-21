using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class RegisterUserDto
{
    [Required] public required string userDisplayName { get; set; }

    [Required] public required string userEmail { get; set; }

    [Required] [MinLength(8)] public required string password { get; set; }

    public DateTime userBirthday { get; set; }
    
    public string? AvatarUrl { get; set; }
}