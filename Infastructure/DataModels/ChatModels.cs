using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Infastructure;

public class ChatModels
{
    
}

public class Rooms
{
    public int rom_id { get; set; }
    public string rom_name { get; set; }
    
}

public class Message
{
    [Required]
    public int room_id { get; set; }
    
    [MinLength(1)]
    [MaxLength(90)]
    public string message { get; set; }
    public bool isSender { get; set; }
    
    [Required]
    public int User_id { get; set; }
    public DateTime sendAt { get; set; }
}