namespace Infastructure;

using Npgsql;

public class Utilities
{ 
    public static readonly string ProperlyFormattedConnectionString;


    static Utilities()
    {
        string rawConnectionString;
        string envVarKeyName = "pgconn";

        rawConnectionString = Environment.GetEnvironmentVariable(envVarKeyName)!;
        ProperlyFormattedConnectionString = rawConnectionString;
    }
}