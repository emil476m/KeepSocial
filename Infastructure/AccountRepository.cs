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
           birthday as {nameof(User.userBirthday)},
           avatarUrl as {nameof(User.avatarUrl)}
           from keepsocial.users where id = @id and isDeleted = false";
        using var connection = _dataSource.OpenConnection();
        return connection.QueryFirst<User>(sql, new { id });
    }

    public bool UpdateUserName(int id, string updatedValue)
    {
        var sql = @$"
UPDATE keepsocial.users SET name = @updatedValue  WHERE id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new {id, updatedValue}) == 1;
        }
    }

    public bool StoreValidation(int userId, int validationNumber)
    {
        var sql =
            $@"INSERT INTO keepsocial.validationnumbers (user_id, validation_number) VALUES(@userId, @validationNumber)";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { userId, validationNumber}) == 1;
        }
    }

    public bool UpdateUserEmail(int id, string updatedValue)
    {
        var sql = @$"
UPDATE keepsocial.users SET email = @updatedValue  WHERE id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new {id, updatedValue}) == 1;
        }
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
           SELECT count(user1_id) from keepsocial.friendRealeatioj 
            where (user1_id = @userId and user2_id = @friendId) 
            OR (user1_id = @friendId and user2_id = @userId);
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new {userId, friendId}) >= 1;
        }
        return false;
    }

    public bool UpdateAvatarImg(int id, string updatedValue)
    {
        var sql = @$"
            UPDATE keepsocial.users SET avatarUrl = @updatedValue  WHERE id = @id";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new {id, updatedValue}) == 1;
        }
    }

    public bool FollowUser(int userId, int followedId)
    {
        var sql =
            $@"INSERT INTO keepsocial.followrelation (followed_id, follower_id) VALUES(@followedid, @userId)";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { userId, followedId }) == 1;
        }
    }
    
    public bool UnFollowUser(int userId, int followedId)
    {
        var sql =
            $@"DELETE FROM keepsocial.followrelation WHERE followed_id = @followedId AND follower_id = @userId;";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { userId, followedId }) == 1;
        }
    }
    
    public bool CheckIfFollowing(int userId, int followedId)
    {
        var sql =
            $@"SELECT count(follower_id) FROM keepsocial.followrelation WHERE followed_id = @followedId AND follower_id = @userId;";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { userId, followedId }) == 1;
        }
    }

    public Profile getProfile(string profileName)
    {
        var userId = 0;
        var sql = $@"select
           id as {nameof(Profile.userId)},
           name as {nameof(Profile.userDisplayName)},
           avatarUrl as {nameof(Profile.avatarUrl)},
           profileDescription as {nameof(Profile.profileDescription)}
           from keepsocial.users where name = @profileName and isDeleted = false;";
        
        var sqlFollowing =
            $@"SELECT count(follower_id) FROM keepsocial.followrelation WHERE follower_id = @userId;";
        
        var sqlFollower =
            $@"SELECT count(followed_id) FROM keepsocial.followrelation WHERE followed_id = @userId;";
        
        using (var conn = _dataSource.OpenConnection())
        {
            var profile =  conn.QueryFirst<Profile>(sql, new { profileName });
            userId = profile.userId;
            Console.WriteLine("this is ther user id: "+userId);
            profile.followers = conn.ExecuteScalar<int>(sqlFollower, new {userId});
            profile.following = conn.ExecuteScalar<int>(sqlFollowing, new {userId});
            return profile;
        }
    }
    
    public void updateAvatar(string? avatarUrl, int userId)
    {
        var sql = @$"
            UPDATE keepsocial.users SET avatarUrl = @avatarUrl  WHERE id = @userId";

        using (var conn = _dataSource.OpenConnection())
        {
            conn.Execute(sql, new {avatarUrl, userId});
        }
    }
}