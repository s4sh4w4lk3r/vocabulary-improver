namespace ViApi.Types.Configuration;

public class DbConfiguration
{
    public string? MySqlConnString { get; init; }
    public string? MongoDbConnString { get; init; }
    public string? MongoDbName { get; init; }
}
