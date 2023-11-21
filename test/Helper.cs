using Dapper;
using Npgsql;

namespace test;

public class Helper
{
    public static readonly NpgsqlDataSource DataSource;
    public static readonly string ClientAppBaseUrl = "http://localhost:4200";
    public static readonly string ApiBaseUrl = "http://localhost:5000/api";

    static Helper()
    {
        var envVarKeyName = "pgconn";

        var rawConnectionString = Environment.GetEnvironmentVariable(envVarKeyName)!;
        try
        {
            var uri = new Uri(rawConnectionString);
            var properlyFormattedConnectionString = string.Format(
                "Server={0};Database={1};User Id={2};Password={3};Port={4};Pooling=false;",
                uri.Host,
                uri.AbsolutePath.Trim('/'),
                uri.UserInfo.Split(':')[0],
                uri.UserInfo.Split(':')[1],
                uri.Port > 0 ? uri.Port : 5432);
            DataSource =
                new NpgsqlDataSourceBuilder(properlyFormattedConnectionString).Build();
            DataSource.OpenConnection().Close();
        }
        catch (Exception e)
        {
            throw new Exception("failed to setup helper class", e);
        }
    }

    public static void TriggerRebuild(string RebuildScript)
    {
        using (var conn = DataSource.OpenConnection())
        {
            try
            {
                conn.Execute(RebuildScript);
            }
            catch (Exception e)
            {
                throw new Exception("failed to rebuild database", e);
            }
        }
    }
}