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
            message as {nameof(Message.message)},
            user_id as {nameof(Message.User_id)},
            CASE WHEN user_id = @userId THEN true ELSE false END as {nameof(Message.isSender)}
            from keepsocial.messages
            
            WHERE rom_id = @romId LIMIT 10 OFFSET @offSetNumber;
        ";
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Message>(sql, new {userId, romId, offSetNumber});
        }
    }
}