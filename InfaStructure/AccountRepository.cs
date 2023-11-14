using Dapper;
using Infastructure;
using Npgsql;

namespace Infastructure;

public class AccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    
    public User CreateUser(string userDisplayName, string userEmail, DateOnly userBirthday)
    {
        var sql =
            $@"INSERT INTO KeepSocial.users (name, email, birthday, isDeleted) VALUES(@userDisplayName, @userEmail,@userBirthday, false) RETURNING 
        id as {nameof(User.userId)}, 
        name as {nameof(User.userDisplayName)},
        email as {nameof(User.userEmail)}, 
        birthday as {nameof(User.userBirthday)},  
        isDeleted as {nameof(User.isDeleted)};";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<User>(sql, new { userDisplayName, userEmail, userBirthday});
        }
    }
}