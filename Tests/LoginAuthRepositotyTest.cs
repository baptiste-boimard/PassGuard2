using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PassGuard.Api.Database;
using PassGuard.Api.Repositories;
using PassGuard.Api.Service;
using PassGuard.Shared.Models;
using Xunit;


namespace Tests;

[Collection("NoParallelTests")]
public class LoginAuthRepositotyTest
{
    private readonly AuthRepository _testAuthRepository;
    private readonly SqliteDbContext _sqliteDbContext;
    
    public LoginAuthRepositotyTest()
    {
        var options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase("TestDbLogin")
            .Options;

        _sqliteDbContext = new SqliteDbContext(options);
        
        _testAuthRepository = new AuthRepository(_sqliteDbContext);
    }
    
    [Fact]
    public async Task Login_Returns_AccountDTO_When_Credentials_Are_Correct()
    {
        // ! Arrange
        // * Mock de la valeur d'entrée
        LoginAccountForm testLoginAccountForm = new LoginAccountForm
        {
            Username = "testuser",
            Password = "testpassword"
        };
        
        // * Mock d'un enregistrement d'un account en BDD
        var salt = "somesalt";
        var pepperKey = PepperKey.LoadPepperKey();

        var passwordBytes = Encoding.UTF8.GetBytes(testLoginAccountForm.Password + salt);
        var computedHash = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var computedHashString = Convert.ToBase64String(computedHash);

        var testAccount = new Account
        {
            Id = new Guid(),
            Username = testLoginAccountForm.Username,
            Password = computedHashString,
            Salt = salt,
        };

        _sqliteDbContext.Accounts.Add(testAccount);
        await _sqliteDbContext.SaveChangesAsync();
        


        // ! Act
        var result = await _testAuthRepository.Login(testLoginAccountForm);
        
        // ! Assert
        Assert.NotNull(result);
        Assert.IsType<Guid>(result.Id);
        Assert.Equal("testuser", result.Username);

    }
    
    [Fact]
    public async Task Login_Entry_LoginAccountForm_Is_Not_In_BDD()
    {
        // ! Arrange
        // * Mock de la valeur d'entrée
        LoginAccountForm testLoginAccountForm = new LoginAccountForm
        {
            Username = "testuser2",
            Password = "testpassword2"
        };
        
        // ! Act
        var result = await _testAuthRepository.Login(testLoginAccountForm);
        
        // ! Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Password_Entry_LoginAccountForm_Is_Not_Correct()
    {
        // ! Arrange
        // * Mock de la valeur d'entrée
        LoginAccountForm testLoginAccountForm = new LoginAccountForm
        {
            Username = "testuser",
            Password = "badtestpassword"
        };
        
        // * Mock d'un enregistrement d'un account en BDD
        var goodPassword = "testpassword";
        var salt = "somesalt";
        var pepperKey = PepperKey.LoadPepperKey();

        var passwordBytes = Encoding.UTF8.GetBytes(goodPassword + salt);
        var computedHash = new HMACSHA256(pepperKey).ComputeHash(passwordBytes);
        var computedHashString = Convert.ToBase64String(computedHash);

        var testAccount = new Account
        {
            Id = new Guid(),
            Username = testLoginAccountForm.Username,
            Password = computedHashString,
            Salt = salt,
            CreatedAt = DateTime.Now
        };

        _sqliteDbContext.Accounts.Add(testAccount);
        await _sqliteDbContext.SaveChangesAsync();
        
        // ! Act
        var result = await _testAuthRepository.Login(testLoginAccountForm);
        
        // ! Assert
        Assert.Null(result);
    }
}