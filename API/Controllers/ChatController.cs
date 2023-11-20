using Infastructure;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;



[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }
    
    [HttpGet("/Rooms")]
    public IEnumerable<Rooms> getAllRooms(int pageNumber)
    {
        //var session = HttpContext.GetSessionData()!;
        int userid = 101;
        int limit = 10;
        return _chatService.getChatRooms(pageNumber, userid);
    }

    [HttpGet("/ChatMessages{roomId}")]
    public IEnumerable<Message> getMessagesInChat([FromRoute] int roomId, int pageNumber)
    {
        //var session = HttpContext.GetSessionData()!;
        int userId = 101;
        return _chatService.getChats(roomId, pageNumber, userId);
    }
    
    [HttpPost("/SenndMessage")]
    public Message snedMessagesInChat([FromBody]Message message)
    {
        //var session = HttpContext.GetSessionData()!;
        int userId = 101;
        return _chatService.sendMessage(message, userId);
    }
}