using System.Data;
using System.Security.Cryptography;
using System.Text;
using PassGuard.Api.Database;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.PepperKey;

// ReSharper disable All

namespace PassGuard.Api.Repositories;

public class DataRepository
{
    public PostgresDbContext _postgresDbContext { get; }

    public DataRepository(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<ObjectPassword?> VerifyExistingOne(ObjectPasswordForm? objectPassword)
    {
        var existingOne = _postgresDbContext.ObjectPasswords
            .FirstOrDefault(
                a => a.Site == objectPassword!.Site &&
                     a.Username == objectPassword.Username)!;

        if (existingOne != null)
        {
            return existingOne;
        }

        return null;
    }

    public async Task<ObjectPasswordDTO> SaveNewObjectPassword(ObjectPasswordForm objectPassword)
    {
        // Création du salt
        var saltBytes = new byte[16];
        RandomNumberGenerator.Create().GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        
        // Récupération de la PepperKey
        var pepperKey = LoadPepperKey();
        
        // Combinaison du salt et du mot de passe
        var passwordBytes = Encoding.UTF8.GetBytes(objectPassword.Password + salt);
        var hashBytes = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var hash = Convert.ToBase64String(hashBytes);
        
        //Création du nouveau ObjectPassword
        var newObjectPassword = new ObjectPassword
        {
            Id = Guid.NewGuid(),
            Site = objectPassword.Site,
            Username = objectPassword.Username,
            Password = hash,
            Category = objectPassword.Category,
            Salt = salt,
            CreatedAt = DateTime.UtcNow
        };

        _postgresDbContext.ObjectPasswords.Add(newObjectPassword);
        _postgresDbContext.SaveChangesAsync();

        var newObjectPasswordDTO = new ObjectPasswordDTO
        {
            Id = newObjectPassword.Id,
            Site = newObjectPassword.Site,
            Username = newObjectPassword.Username,
            Password = newObjectPassword.Password,
            Category = newObjectPassword.Category,
            CreatedAt = newObjectPassword.CreatedAt
        };

        return newObjectPasswordDTO;
    }

    public async Task<ObjectPassword[]> GetPasswords()
    {
        ObjectPassword[] passwordArray = _postgresDbContext.ObjectPasswords.ToArray();

        if (passwordArray == null)
        {
            return null;
        }

        return passwordArray;
    }
    
    
}