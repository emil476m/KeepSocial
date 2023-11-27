using Dapper;
using Npgsql;


namespace Infastructure;

public class AccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }


    public User CreateUser(string userDisplayName, string userEmail, DateTime userBirthday)
    {
        var sql =
            $@"INSERT INTO keepsocial.users (name, email, birthday, isDeleted) VALUES(@userDisplayName, @userEmail,@userBirthday, false) RETURNING 
        id as {nameof(User.userId)}, 
        name as {nameof(User.userDisplayName)},
        email as {nameof(User.userEmail)}, 
        birthday as {nameof(User.userBirthday)},  
        isDeleted as {nameof(User.isDeleted)};";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<User>(sql, new { userDisplayName, userEmail, userBirthday });
        }
    }

    public IEnumerable<User> getUserName()
    {
        var sql =
            $@"SELECT  
        id as {nameof(User.userId)}, 
        name as {nameof(User.userDisplayName)},
        email as {nameof(User.userEmail)}, 
        birthday as {nameof(User.userBirthday)}
        FROM keepsocial.users WHERE isDeleted = false
       ;";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<User>(sql);
        }
    }

    public User? GetById(int id)
    {
        var sql = $@"select
           id as {nameof(User.userId)},
           name as {nameof(User.userDisplayName)},
           email as {nameof(User.userEmail)},
           birthday as {nameof(User.userBirthday)}
           from keepsocial.users where id = @id and isDeleted = false";
        using var connection = _dataSource.OpenConnection();
        return connection.QueryFirst<User>(sql, new { id });
    }

    public IEnumerable<User> getFriends(int userId, int offSetNumber)
    {
        var sql = $@"
        select 
            id as {nameof(User.userId)},
            name as {nameof(User.userDisplayName)},
            email as {nameof(User.userEmail)},
            birthday as {nameof(User.userBirthday)}
            
        from keepsocial.users
        where id != @userId
        and id in (select distinct u.id
        from keepsocial.users as u
        join keepsocial.friendRealeatioj as f on u.id = f.user1_id or u.id = f.user2_id
        where (f.user1_id = @userId or f.user2_id = @userId))
        LIMIT 10 OFFSET @offSetNumber;
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<User>(sql, new {userId, offSetNumber});
        }
    }

    public bool isFriends(int userId, int friendId)
    {
        var sql = $@"
           SELECT user1_id from keepsocial.friendRealeatioj 
            where (user1_id = @usserID and user2_id = @friendId) 
            OR (user1_id = @friendId and user2_id = userId);
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            int number = conn.QuerySingle<int>(sql, new {userId, friendId});
            if (number == userId || number == friendId) return true;
        }
        return false;
    }
}