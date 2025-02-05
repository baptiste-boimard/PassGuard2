namespace PassGuard.Api.Service;

public class JwtKey
{
    public static dynamic LoadJwtKey()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        var jwtKey = new
        {
            jwtSecret = config["Jwt:jwtSecret"],
            issuer = config["Jwt:issuer"],
            audience = config["Jwt:audience"],
        };

        return jwtKey;
    }
}
