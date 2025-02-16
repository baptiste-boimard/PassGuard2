using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PassGuard.Api.Database;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;
using static PassGuard.Api.Service.PepperKey;


namespace PassGuard.Api.Repositories;

public class AuthRepository
{
    private readonly PostgresDbContext _postgresDbContext;

    public AuthRepository(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
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
        
        _postgresDbContext.Accounts.Add(newAccount);
        _postgresDbContext.SaveChangesAsync();

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
        var existingAccount = await _postgresDbContext.Accounts
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
        var existingAccount = await _postgresDbContext.Accounts
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
}