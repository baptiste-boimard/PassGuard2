using System.Data;
using System.Security.Cryptography;
using System.Text;
using PassGuard.Api.Database;
using PassGuard.Api.Service;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.PepperKey;

// ReSharper disable All

namespace PassGuard.Api.Repositories;

public class DataRepository
{
    public SqliteDbContext _sqliteDbContext { get; }

    public DataRepository(SqliteDbContext sqliteDbContext)
    {
        _sqliteDbContext = sqliteDbContext;
    }

    public async Task<ObjectPassword?> VerifyExistingOne(ObjectPasswordForm? objectPassword)
    {
        var existingOne = _sqliteDbContext.ObjectPasswords
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
        var cryptedPass = AESService.EncryptString(objectPassword.Password);
        
        //Création du nouveau ObjectPassword
        var newObjectPassword = new ObjectPassword
        {
            Id = Guid.NewGuid(),
            Site = objectPassword.Site,
            Username = objectPassword.Username,
            Password = cryptedPass,
            Category = objectPassword.Category,
            Salt = cryptedPass,
            CreatedAt = DateTime.UtcNow
        };

        _sqliteDbContext.ObjectPasswords.Add(newObjectPassword);
        _sqliteDbContext.SaveChangesAsync();

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
        ObjectPassword[] passwordArray = _sqliteDbContext.ObjectPasswords.ToArray();

        if (passwordArray == null)
        {
            return null;
        }

        return passwordArray;
    }

    public async Task<ObjectPassword> PatchPassword(Guid id, ObjectPassword objectPassword)
    {
        var modifiedPassword = await _sqliteDbContext.ObjectPasswords.FindAsync(id);

        if (modifiedPassword == null) return null;
        
        // Si c'est un nouveau mot de passe on le crypte
        if (objectPassword.Password != modifiedPassword.Password)
        {
            var cryptedPass = AESService.EncryptString(objectPassword.Password);
            objectPassword.Password = cryptedPass;
        }
        
        _sqliteDbContext.Entry(modifiedPassword).CurrentValues.SetValues(objectPassword);
        
        await _sqliteDbContext.SaveChangesAsync();

        return modifiedPassword;
    }

    public async Task<ObjectPassword> DeletePassword(Guid id)
    {
        var deletedPassword =
            await _sqliteDbContext.ObjectPasswords.FindAsync(id);

        if (deletedPassword == null) return null;
        
        _sqliteDbContext.ObjectPasswords.Remove( deletedPassword);
        await _sqliteDbContext.SaveChangesAsync();

        return deletedPassword;
    }
}