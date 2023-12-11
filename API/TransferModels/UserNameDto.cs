using System.ComponentModel.DataAnnotations;

namespace API.TransferModels;

public class UserNameDto
{
    
    [MaxLength(500)]
    [Required]
    public required string profileDescription { get; set; }
}