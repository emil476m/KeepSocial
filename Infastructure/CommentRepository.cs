using Npgsql;

namespace Infastructure;

public class CommentRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CommentRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
}