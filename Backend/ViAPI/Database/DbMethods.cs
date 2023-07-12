using ViAPI.Database;

namespace ViAPI.Database;

public static class DbMethods
{
    public static bool CheckConnection() => new ViDbContext().Database.CanConnect();
}

