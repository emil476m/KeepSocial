using API.Filters;
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
    [RequireAuthentication]
    public IEnumerable<Rooms> getAllRooms(int pageNumber)
    {
        var session = HttpContext.GetSessionData()!;
        int userid = session.UserId;
        int limit = 10;
        return _chatService.getChatRooms(pageNumber, userid);
    }

    [HttpGet("/ChatMessages{roomId}")]
    [RequireAuthentication]
    public IEnumerable<Message> getMessagesInChat([FromRoute] int roomId, int pageNumber)
    {
        var session = HttpContext.GetSessionData()!;
        int userid = session.UserId;
        return _chatService.getChats(roomId, pageNumber, userid);
    }
    
    [HttpPost("/SenndMessage")]
    [RequireAuthentication]
    public Message snedMessagesInChat([FromBody]Message message)
    {
        var session = HttpContext.GetSessionData()!;
        int userId = 111;
        return _chatService.sendMessage(message, userId);
    }
}