using Infastructure;

namespace Service;

public class ChatService
{
    private readonly ChatReposetory _chatReposetory;

    public ChatService(ChatReposetory chatReposetory)
    {
        _chatReposetory = chatReposetory;
    }

    public IEnumerable<Message> getChats(int roomId, int pageNumber, SessionData data)
    {
        try
        {
            int offset = (10 * pageNumber)-10;

            return _chatReposetory.getChats(roomId, offset, data.UserId).Reverse();
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
            return _chatReposetory.sendMessage(message.room_id, message.message,data.UserId);
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
}