using System.Collections;

namespace Infastructure;

public class ChatModels
{
    
}

public class Rooms
{
    public int rom_id { get; set; }
    public string rom_name { get; set; }
    //public User ArrayList { get; set; }
    
}

public class Message
{
    public int room_id { get; set; }
    public string message { get; set; }
    public bool isSender { get; set; }
    public int User_id { get; set; }
    public DateTime sendAt { get; set; }
}