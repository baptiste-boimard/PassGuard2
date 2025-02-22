using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ObjectPasswordDTO> SaveNewObjectPassword(CreatePassword objectPassword)
    {
        var cryptedPass = AESService.EncryptString(objectPassword.ObjectPasswordForm.Password);

        var unhashedToken = new JwtSecurityTokenHandler().ReadJwtToken(objectPassword.Token);
        var subClaim = unhashedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (subClaim == null) return null;

        if (!Guid.TryParse(subClaim.Value, out Guid userId)) return null;

        //Création du nouveau ObjectPassword
        var newObjectPassword = new ObjectPassword
        {
            Id = Guid.NewGuid(),
            Site = objectPassword.ObjectPasswordForm.Site,
            Username = objectPassword.ObjectPasswordForm.Username,
            Password = cryptedPass,
            Category = objectPassword.ObjectPasswordForm.Category,
            CreatedAt = DateTime.UtcNow,
            AccountId = userId
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
            CreatedAt = newObjectPassword.CreatedAt,
            AccountId = newObjectPassword.AccountId
        };

        return newObjectPasswordDTO;
    }

    public async Task<ObjectPassword[]> GetPasswords(string token)
    {
        // Décodage du token
        var unhashedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var subClaim = unhashedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (subClaim == null || string.IsNullOrWhiteSpace(subClaim.Value)) return Array.Empty<ObjectPassword>();
        
        if (!Guid.TryParse(subClaim.Value, out Guid userId))
        {
            return Array.Empty<ObjectPassword>();
        }

        var passwordArray = await _sqliteDbContext.ObjectPasswords
            .Where(o => o.AccountId == userId)
            .ToArrayAsync();

        if (passwordArray == null)
        {
            return null;
        }

        return passwordArray;
    }

    public async Task<ObjectPassword> PatchPassword(Guid id, UpdatePassword objectPassword)
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