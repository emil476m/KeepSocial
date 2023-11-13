namespace Service;

public class SessionData
{
    
    public static SessionData FromDictionary(Dictionary<string, object> dict)
    {
        return new SessionData {};
    }
    
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object> {};
    }
}

public static class Keys
{
}