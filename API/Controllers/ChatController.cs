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
    
    [HttpGet("/Rooms{userId}")]
    public IEnumerable<Rooms> getAllRooms([FromRoute]int userID, int offSet)
    {
        //var session = HttpContext.GetSessionData()!;
        int limit = 10;
        throw new NotImplementedException();
    }

    [HttpGet("/ChatRoom{roomId}")]
    public IEnumerable<Message> getMessagesInChat([FromRoute] int roomId, int pageNumber, int userId)
    {
        //var session = HttpContext.GetSessionData()!;
        return _chatService.getChats(roomId, pageNumber, userId);
    }
}