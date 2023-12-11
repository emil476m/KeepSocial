using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class RegisterUserDto
{
    [MaxLength(100)]
    [MinLength(1)]
    [Required] public required string userDisplayName { get; set; }

    [EmailAddress]
    [Required] public required string userEmail { get; set; }

    
    [MaxLength(40)]
    [MinLength(8)]
    [Required] public required string password { get; set; }
    
    public DateTime userBirthday { get; set; }
    
    public string? AvatarUrl { get; set; }
}