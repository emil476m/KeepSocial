using Infastructure;

namespace Service;

public class SessionData
{

    public required int UserId { get; set; }

    public static SessionData FromUser(User user)
    {
        return new SessionData { UserId = user.userId };
    }

    public static SessionData FromDictionary(Dictionary<string, object> dict)
    {
        return new SessionData { UserId = (int)dict[Keys.UserId] };
    }
    
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object> {{Keys.UserId,UserId}};
    }
}

public static class Keys
{
    public const string UserId = "u";
}