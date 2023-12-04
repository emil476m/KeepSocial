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
    
    [RequireAuthentication]
    [HttpGet("/api/Rooms")]
    public IEnumerable<Rooms> getAllRooms(int pageNumber)
    {
        var session = HttpContext.GetSessionData()!;
        return _chatService.getChatRooms(pageNumber, session);
    }

    [RequireAuthentication]
    [HttpGet("/api/ChatMessages{roomId}")]
    public IEnumerable<Message> getMessagesInChat([FromRoute] int roomId, int pageNumber)
    {
        var session = HttpContext.GetSessionData()!;
        
        return _chatService.getChats(roomId, pageNumber, session);
    }
    
    [RequireAuthentication]
    [HttpPost("/api/SenndMessage")]
    public Message snedMessagesInChat([FromBody]Message message)
    {
        var session = HttpContext.GetSessionData()!;
        return _chatService.sendMessage(message, session);
    }
    
    [RequireAuthentication]
    [HttpGet("/api/friendChat{friendId}")]
    public Rooms getMessagesInChat([FromRoute] int friendId)
    {
        var session = HttpContext.GetSessionData()!;
        
        return _chatService.getFriendRoom(friendId, session);
    }
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/account/GetFriendRequests")]
    public IEnumerable<User> GetFriendRequests()
    {
        int userId = HttpContext.GetSessionData().UserId!;
        throw new NotImplementedException();
        //TODO get friend Request to User, and only this user
    }
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/account/FriendRequestsResponse")]
    public IEnumerable<User> FriendRequestsResponse(RequestUpdateDto response)
    {
        int userId = HttpContext.GetSessionData().UserId!;
        throw new NotImplementedException();
        //TODO Make the bool in the DTO be the decline or accept for request
    }
}