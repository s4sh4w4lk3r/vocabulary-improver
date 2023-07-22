using Microsoft.EntityFrameworkCore;

namespace ViAPI.Other;

public static class ViConfiguration
{
    public enum SecretType
    {
        MySql,
        TelegramToken,
        JWTKey
    }
    public static string GetSecretString(SecretType type)
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        string secretsPath = builder.GetSection("ViApiSettings")["SecretsPath"]!;
        InputChecker.CheckStringException(secretsPath);
        builder = new ConfigurationBuilder().AddJsonFile(secretsPath).Build();

        switch (type)
        {
            case SecretType.MySql:
                string connstring = builder.GetConnectionString("MySql")!;
                InputChecker.CheckStringException(connstring);
                return connstring;

            case SecretType.JWTKey:
                string key = builder.GetRequiredSection("JWT")["SecurityKey"]!;
                InputChecker.CheckStringException(key);
                return key;
        }
        return null!;
    }
    public static DbContextOptions GetDatabaseOptions(DbContextOptionsBuilder optionsBuilder)
    {

        string connstring = GetSecretString(SecretType.MySql);

        optionsBuilder.UseMySql(connstring, ServerVersion.AutoDetect(connstring));

        optionsBuilder.UseLazyLoadingProxies(true);

        return optionsBuilder.Options;
    }
}

