using Infastructure;

namespace Service;

public class ChatService
{
    private readonly ChatReposetory _chatReposetory;

    public ChatService(ChatReposetory chatReposetory)
    {
        _chatReposetory = chatReposetory;
    }

    public IEnumerable<Message> getChats(int roomId, int pageNumber, int userId)
    {
        try
        {
            int offset = (10 * pageNumber)-10;
            return _chatReposetory.getChats(roomId, offset, userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("could not fetch chat data");
        }
    }
}