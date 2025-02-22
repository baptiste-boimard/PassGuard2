namespace PassGuard.Api.Service;

public class AESKey
{
    public static string LoadAESKey()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var AESKey = config["Secrets:AESKey"];

        return AESKey;
    } 
}