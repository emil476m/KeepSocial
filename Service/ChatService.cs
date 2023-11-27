using Infastructure;

namespace Service;

public class ChatService
{
    private readonly ChatReposetory _chatReposetory;
    private readonly AccountRepository _accountRepository;

    public ChatService(ChatReposetory chatReposetory, AccountRepository accountRepository)
    {
        _chatReposetory = chatReposetory;
        _accountRepository = accountRepository;
    }

    public IEnumerable<Message> getChats(int roomId, int pageNumber, SessionData data)
    {
        try
        {
            if (_chatReposetory.isUserInRoom(roomId, data.UserId))
            {
                int offset = (10 * pageNumber)-10;
                return _chatReposetory.getChats(roomId, offset, data.UserId).Reverse();
            }

            throw new Exception("you do not have permision for this action");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not fetch chat data");
        }
    }

    public Message sendMessage(Message message, SessionData data)
    {
        try
        {
            if (_chatReposetory.isUserInRoom(message.room_id, data.UserId))
            {
                return _chatReposetory.sendMessage(message.room_id, message.message,data.UserId);
            }

            throw new Exception("you do not have permision for this action");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not send message");
        }
    }

    public IEnumerable<Rooms> getChatRooms(int pageNumber, SessionData data)
    {
        try
        {
            int offset = (10 * pageNumber)-10;
            return _chatReposetory.getChatRoooms( offset, data.UserId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not fetch chat data");
        }
    }

    public int getFriendRoom(int friendId, SessionData data)
    {
        try
        {
            int userId = data.UserId;
            
            if (!_accountRepository.isFriends(userId,friendId)) throw new Exception("The two of you arent even friends"); 
            

            int roomId = _chatReposetory.friedUserCHatRoom(userId, friendId);

            if (roomId != -1) return roomId;

            //TODO Create Room if it dossent exist
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not fetch chat data");
        }
    }
}