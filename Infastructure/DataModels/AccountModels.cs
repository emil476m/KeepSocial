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