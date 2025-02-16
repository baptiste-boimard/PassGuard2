using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.JwtKey;

namespace PassGuard.Api.Service;

public class JwtService
{
    public static  string JwtCreateToken(AccountDTO account)
    {
        // Récupération des keys pour le jwt
        var jwtKey = LoadJwtKey();


        var claims = new List<Claim>
        {
            // Le claim "sub" pour l'identifiant de l'utilisateur
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),

            // Le claim "unique_name" pour le nom d'utilisateur
            new Claim(JwtRegisteredClaimNames.UniqueName, account.Username),

            // Si vous souhaitez inclure la date de création, vous pouvez ajouter un claim personnalisé
            new Claim("CreatedAt", account.CreatedAt.ToString("o"))
        };

        var token = new JwtSecurityToken(
            issuer: jwtKey.issuer,
            audience: jwtKey.audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey.jwtSecret)),
                SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}