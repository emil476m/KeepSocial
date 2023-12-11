using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Infastructure;

public class AccountModels
{
}

public class User
{
    public int userId { get; set; }
    
    [MaxLength(100)]
    [MinLength(1)]
    public string userDisplayName { get; set; }
    
    [EmailAddress]
    public string userEmail { get; set; }
    public DateTime userBirthday { get; set; }
    
    public string? avatarUrl { get; set; }
    
    public bool isDeleted { get; set; }
}

public class Profile
{
    public int userId { get; set; }
    
    [MaxLength(100)]
    [MinLength(1)]
    public string userDisplayName { get; set; }
    public string? avatarUrl { get; set; }
    
    [MaxLength(500)]
    public string profileDescription { get; set; } 
    public int postAmount { get; set; }
    public int followers { get; set; }
    public int following { get; set; }
    public bool isFriend { get; set; }
    public bool isFollowing { get; set; }
    public bool isSelf { set; get; }
}