using Dapper;
using Infastructure;
using Newtonsoft.Json;
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


    public static object? checkifexists(int id, string sql)
    {
        using (var conn = DataSource.OpenConnection())
        {
            try
            {
                return conn.ExecuteScalar<int>(sql, new{id});
            }
            catch (Exception e)
            {
                throw new Exception("Failed to check if a comment exists", e);
            }
        }
    }

    public static string NoResponseMessage = $@"
failed to get a response from the API.
";
    
    public static string BadResponseBody(string content)
    {
        return $@"
RESPONSE BODY: {content}

EXCEPTION:
";
    }
    
    public static string MyBecause(object actual, object expected)
    {
        string expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        string actualJson = JsonConvert.SerializeObject(actual, Formatting.Indented);

        return $"because we want these objects to be equivalent:\nExpected:\n{expectedJson}\nActual:\n{actualJson}";
    }
}