using Dapper;
using Npgsql;

namespace Infastructure;

public class PasswordHashRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PasswordHashRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    public PasswordHash GetByEmail(string email)
    {
        const string sql = $@"
SELECT 
    user_id as {nameof(PasswordHash.UserId)},
    hash as {nameof(PasswordHash.Hash)},
    salt as {nameof(PasswordHash.Salt)},
    algorithm as {nameof(PasswordHash.Algorithm)}
FROM keepsocial.password_hash
JOIN keepsocial.users ON keepsocial.password_hash.user_id = keepsocial.users.id
WHERE email = @email;
";
        using var connection = _dataSource.OpenConnection();
        return connection.QuerySingle<PasswordHash>(sql, new { email });
    }
    
    public void Create(int userId, string hash, string salt, string algorithm)
    {
        const string sql = $@"
INSERT INTO keepsocial.password_hash (user_id, hash, salt, algorithm)
VALUES (@userId, @hash, @salt, @algorithm)
";
        using var connection = _dataSource.OpenConnection();
        connection.Execute(sql, new { userId, hash, salt, algorithm });
    }

    public bool Update(int userId, string hash, string salt, string algorithm)
    {
        const string sql = $@"
UPDATE keepsocial.password_hash
SET hash = @hash, salt = @salt, algorithm = @algorithm
WHERE user_id = @userId
";
        using var connection = _dataSource.OpenConnection();
        return connection.Execute(sql, new { userId, hash, salt, algorithm }) == 1;
    }
}