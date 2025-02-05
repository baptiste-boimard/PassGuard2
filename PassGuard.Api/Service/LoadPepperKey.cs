namespace PassGuard.Api.Service;

public class PepperKey
{
    public static Byte[] LoadPepperKey()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var pepperKeyString = config["Secrets:PepperKey"];
        var pepperKey = Convert.FromBase64String(pepperKeyString!);
        
        return pepperKey;
    }
}