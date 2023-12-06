using Infastructure;

namespace Service;

public class SessionData
{

    public required int UserId { get; set; }

    /*
     * get data from a user object
     */
    public static SessionData FromUser(User user)
    {
        return new SessionData { UserId = user.userId };
    }

    /*
     * get data from a dictionary
     */
    public static SessionData FromDictionary(Dictionary<string, object> dict)
    {
        return new SessionData { UserId = (int)dict[Keys.UserId] };
    }
    
    /*
     * adds data to a dictionary
     */
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object> {{Keys.UserId,UserId}};
    }
}

public static class Keys
{
    public const string UserId = "u";
}