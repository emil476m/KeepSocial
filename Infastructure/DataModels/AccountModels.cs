using System.Runtime.InteropServices.JavaScript;

namespace Infastructure;

public class AccountModels
{
}

public class User
{
    public int userId { get; set; }
    public string userDisplayName { get; set; }
    public string userEmail { get; set; }
    public DateTime userBirthday { get; set; }
    
    public string? avatarUrl { get; set; }
    
    public bool isDeleted { get; set; }
}

public class Profile
{
    public int userId { get; set; }
    public string userDisplayName { get; set; }
    public string? avatarUrl { get; set; }
    public string profileDescription { get; set; } 
    public int postAmount { get; set; }
    public int followers { get; set; }
    public int following { get; set; }
    public bool isFriend { get; set; }
    public bool isFollowing { get; set; }
    public bool isSelf { set; get; }
}