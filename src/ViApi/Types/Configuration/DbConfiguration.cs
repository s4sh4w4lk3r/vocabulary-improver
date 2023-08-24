namespace ViApi.Types.Configuration;

public class DbConfiguration
{
    private string mySqlConnString = null!;
    private string mongoDbConnString = null!;
    private string mongoDbName = null!;

    public required string MySqlConnString { get => mySqlConnString; init => mySqlConnString = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string MongoDbConnString { get => mongoDbConnString; init => mongoDbConnString = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string MongoDbName { get => mongoDbName; init => mongoDbName = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
}
