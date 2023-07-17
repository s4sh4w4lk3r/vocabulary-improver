namespace ViAPI.StaticMethods;

public static class ViConfiguration
{
    public enum SecretType
    {
        MySql,
        TelegramToken, 
        APIHostname,
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

            case SecretType.TelegramToken:
                string token = builder.GetRequiredSection("Tokens")["TelegramBot"]!;
                InputChecker.CheckStringException(token);
                return token;

            case SecretType.APIHostname:
                string hostname = builder.GetRequiredSection("Hostnames")["API"]!;
                InputChecker.CheckStringException(hostname);
                return hostname;

            case SecretType.JWTKey:
                string key = builder.GetRequiredSection("JWT")["SecurityKey"]!;
                InputChecker.CheckStringException(key);
                return key;
        }
        return null!;
    }
}
