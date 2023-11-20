using Dapper;
using Npgsql;

namespace Infastructure;

public class ChatReposetory
{
    private readonly NpgsqlDataSource _dataSource;

    public ChatReposetory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Message> getChats(int romId, int offSetNumber, int userId)
    {
        var sql = $@"
            select 
            rom_id as {nameof(Message.room_id)},
            message as {nameof(Message.message)},
            user_id as {nameof(Message.User_id)},
            time_Send as {nameof(Message.sendAt)},
            CASE WHEN user_id = @userId THEN true ELSE false END as {nameof(Message.isSender)}
            from keepsocial.messages
            
            WHERE rom_id = @romId LIMIT 10 OFFSET @offSetNumber;
        ";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Message>(sql, new {userId, romId, offSetNumber});
        }
    }

    public Message sendMessage(int roomId, string message, int userId)
    {
        var sql = $@"
            INSERT INTO keepsocial.messages(rom_id, user_id, message, time_Send) 
            VALUES (@roomId, @userId, @message, NOW())
            RETURNING
            rom_id as {nameof(Message.room_id)},
            message as {nameof(Message.message)},
            user_id as {nameof(Message.User_id)},
            time_Send as {nameof(Message.sendAt)},
            CASE WHEN user_id = @userId THEN true ELSE false END as {nameof(Message.isSender)}
        ";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Message>(sql, new {roomId, userId, message});
        }
    }

    public IEnumerable<Rooms> getChatRoooms(int offset, int userId)
    { 
        var sql = $@"
            select 
            keepsocial.chatrooms.rom_id as {nameof(Rooms.rom_id)},
            rom_name as {nameof(Rooms.rom_name)}
            FROM keepsocial.chatrooms JOIN keepsocial.chatroomUsersRealation ON chatroomUsersRealation.rom_id = chatrooms.rom_id
            WHERE user_id = @userId
            LIMIT 10 OFFSET @offset;
        ";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Rooms>(sql, new {userId, offset});
        }
    }
}