using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PassGuard.Api.Database;
using PassGuard.Api.Service;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.PepperKey;


namespace PassGuard.Api.Repositories;

public class AuthRepository
{
    private readonly SqliteDbContext _sqliteDbContext;
    // private readonly PostgresDbContext _sqliteDbContext;

    public AuthRepository(SqliteDbContext sqliteDbContext)
    {
        _sqliteDbContext = sqliteDbContext;
        // _sqliteDbContext = postgresDbContext;
    }

    public async Task<AccountDTO> SaveNewAccount(RegisterAccountForm registerAccountForm)
    {
        // Création du salt
        var saltBytes = new byte[16];
        RandomNumberGenerator.Create().GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        
        // Récupération de la PepperKey
        var pepperKey = LoadPepperKey();
        
        // Combinaison du salt et du mot de passe
        var passwordBytes = Encoding.UTF8.GetBytes(registerAccountForm.Password + salt);
        var hashBytes = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var hash = Convert.ToBase64String(hashBytes);
        
        // Création du nouvel Account
        var newAccount = new Account
        {
            Id = Guid.NewGuid(),
            Username = registerAccountForm.Username,
            Password = hash,
            Salt = salt,
            CreatedAt = DateTime.UtcNow
        };
        
        _sqliteDbContext.Accounts.Add(newAccount);
        _sqliteDbContext.SaveChangesAsync();

        var newAccountDTO = new AccountDTO
        {
            Id = newAccount.Id,
            Username = newAccount.Username,
            CreatedAt = newAccount.CreatedAt,
        };
        
        return newAccountDTO;
    }

    public async Task<Account?> VerifyExistingAccount(RegisterAccountForm registerAccountForm)
    {
        // Recherche si un compte avec le même Username existe déjà
        var existingAccount = await _sqliteDbContext.Accounts
            .FirstOrDefaultAsync(
                a => a.Username == registerAccountForm.Username);

        if (existingAccount != null)
        {
            return existingAccount;
        }

        return null;
    }

    public async Task<AccountDTO> Login(LoginAccountForm loginAccountForm)
    {
        // Recherche de l'utilisateur et comparaison du mot de passe
        var existingAccount = await _sqliteDbContext.Accounts
            .FirstOrDefaultAsync(
                a => a.Username == loginAccountForm.Username);
        if (existingAccount == null)
        {
            return null;
        }
        
        // Récupération de la PepperKey
        var pepperKey = LoadPepperKey();
        
        // Comparaison des hash entre eux
        var passwordBytes = Encoding.UTF8.GetBytes(loginAccountForm.Password + existingAccount.Salt);
        var computedHash  = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var computedHashString  = Convert.ToBase64String(computedHash);
        
        if (existingAccount.Password != computedHashString)
        {
            return null;
        }

        var existingAccountDTO = new AccountDTO
        {
            Id = existingAccount.Id,
            Username = existingAccount.Username,
            CreatedAt = existingAccount.CreatedAt,
        }; 
        
        return existingAccountDTO;
    }

    public async Task<string> VerifyPassword(VerifyPassword payload)
    {
        // Décodage du token
        var token = new JwtSecurityTokenHandler().ReadJwtToken(payload.Token);
        var subClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (subClaim == null) return null;
        
        // Recherche du user avec son id
        if (!Guid.TryParse(subClaim.Value, out Guid userId)) return null;
        
        var user = _sqliteDbContext.Accounts.Find(userId);
        
        if (user == null) return null;
        
        // Récupération de la PepperKey
        var pepperKey = LoadPepperKey();
        
        // Comparaison des hash entre eux
        var passwordBytes = Encoding.UTF8.GetBytes(payload.Password + user.Salt);
        var computedHash  = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var computedHashString  = Convert.ToBase64String(computedHash);
        
        if (user.Password == computedHashString)
        {
            // Récupération de l'ObjectPassword
            var objectPassword = _sqliteDbContext.ObjectPasswords.Find(payload.IdLine);
            
            // Decryptage du mot de passe
            try
            {
                var decryptedResult = AESService.DecryptString(objectPassword.Password);
                
                return decryptedResult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        return null;
    }

    public async Task<string> GetEmail(string token)
    {
        var unhashedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        
        var subClaim = unhashedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        
        if (subClaim == null) return null;
        
        return subClaim.Value;
    }
}