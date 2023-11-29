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
            
            WHERE rom_id = @romId 
            ORDER BY time_Send DESC
            LIMIT 10 OFFSET @offSetNumber;
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Message>(sql, new { userId, romId, offSetNumber });
        }
    }

    public bool isUserInRoom(int roomId, int userId)
    {
        var sql = $@"
            SELECT rom_id from keepsocial.chatroomUsersRealation where rom_id = @roomId AND user_id = @userId;
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            int number = conn.QuerySingle<int>(sql, new { roomId, userId });
            
            if (number == roomId) return true;
        }

        return false;
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
            return conn.QueryFirst<Message>(sql, new { roomId, userId, message });
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
            return conn.Query<Rooms>(sql, new { userId, offset });
        }
    }

    public int friedUserCHatRoom(int userId, int friendId)
    {
        List<int> roomsListID = new List<int>();
        
        var sql1 = $@"
            SELECT b.rom_id
            FROM keepsocial.chatroomusersrealation as b
            JOIN keepsocial.chatroomusersrealation as c ON c.rom_id = b.rom_id
            WHERE b.user_id = @userId AND c.user_id = @friendId;
        ";

        var sql2 = $@"
        SELECT user_id FROM keepsocial.chatroomUsersRealation WHERE rom_id = @roomId;
        ";
        

        using (var conn = _dataSource.OpenConnection())
        {
            roomsListID = (List<int>)conn.Query<int>(sql1, new { userId, friendId });

            foreach (int roomId in roomsListID)
            {
                List<int> amountOfUsers =  (List<int>)conn.Query<int>(sql2, new { roomId });
                if (amountOfUsers.Count == 2) return roomId;
            }
        }

        return -1;
    }

    public int createChatroomWithFirend(int userId, int friendId)
    {
        var sql1 = $@"
            INSERT INTO keepsocial.chatrooms(rom_name) 
            values ((Select name FROM keepsocial.users WHERE keepsocial.users.id = @userId) 
                || ' ' || 
                (Select name FROM keepsocial.users WHERE keepsocial.users.id = @friendId))
            returning rom_id;
        ";

        var sql2 = $@"
                INSERT INTO keepsocial.chatroomUsersRealation(rom_id, user_id)
                values (@roomId, @personId);
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            var transaction = conn.BeginTransaction();
            try
            {
                int roomId = conn.QuerySingle<int>(sql1, new {userId,friendId});

                int personId = userId;
                conn.Query(sql2, new { roomId, personId });
                personId = friendId;
                conn.Query(sql2, new { roomId, personId });
                
                transaction.Commit();
                return roomId;
            }catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }
    
    public Rooms getSingleRooom(int roomId)
    {
        var sql = $@"
            select 
            keepsocial.chatrooms.rom_id as {nameof(Rooms.rom_id)},
            rom_name as {nameof(Rooms.rom_name)}
            FROM keepsocial.chatrooms
            WHERE rom_id = @roomId
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Rooms>(sql, new { roomId});
        }
    }
}