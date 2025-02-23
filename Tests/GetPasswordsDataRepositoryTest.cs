using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PassGuard.Api.Database;
using PassGuard.Api.Repositories;
using PassGuard.Api.Service;
using PassGuard.Shared.DTO;
using PassGuard.Shared.Models;

namespace Tests;

public class GetPasswordsDataRepositoryTest
{
    private readonly DataRepository _dataRepository;
    private readonly SqliteDbContext _sqliteDbContext;

    public GetPasswordsDataRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseInMemoryDatabase("TestDbGetPassword")
            .Options;

        _sqliteDbContext = new SqliteDbContext(options);

        _dataRepository = new DataRepository(_sqliteDbContext);
    }

    [Fact]
    public async Task GetPasswords_Returns_ObjectPassword_Array_With_Good_Token()
    {
        //! Arrange
        //* Mock d'ObjectPasswords pour simuler la BDD
        var userId = Guid.NewGuid();

        var objectPasswords = new[]
        {
            new ObjectPassword
            {
                Id = Guid.NewGuid(),
                Site = "Site1",
                Username = "Toto",
                Password = "Toto",
                Category = "Category1",
                CreatedAt = DateTime.Now,
                AccountId = userId
            },
            new ObjectPassword
            {
                Id = Guid.NewGuid(),
                Site = "Site2",
                Username = "Tata",
                Password = "Tata",
                Category = "Category2",
                CreatedAt = DateTime.Now,
                AccountId = userId
            }
        };
        _sqliteDbContext.ObjectPasswords.AddRange(objectPasswords);
        await _sqliteDbContext.SaveChangesAsync();
        
        //* Mock de la valeur d'entrée
        var token = JwtService.JwtCreateToken(new AccountDTO
        {
            Id = userId,
            Username = "Toto",
            CreatedAt = DateTime.Now
        });
        
        //! Act
        var result = _dataRepository.GetPasswords(token).Result;
        
        //! Assert
        Assert.Equal(2, result.Length);
        Assert.IsType<Guid>(result[0].Id);
        Assert.Equal("Site1",  result[0].Site);
        Assert.Equal("Toto",  result[0].Username);
        Assert.Equal("Toto",  result[0].Password);
        Assert.Equal("Category1",  result[0].Category);
        Assert.Equal(objectPasswords[0].CreatedAt, result[0].CreatedAt);
        Assert.Equal(userId, result[0].AccountId);
    }
    
    [Fact]
    public async Task GetPasswords_Returns_ObjectPassword_Array_Empty_With_Bad_Token()
    {
        //! Arrange
        //* Mock de la valeur d'entrée
        var token = new JwtSecurityToken(
            claims: new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, "notaguid")
            }
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        //! Act
        var result = _dataRepository.GetPasswords(tokenString).Result;
        
        //! Assert
        Assert.Equal(0, result.Length);
    }
    
    [Fact]
    public async Task GetPasswords_Returns_No_ObjectPassword_Array_With_Bad_AccountId()
    {
        //! Arrange
        //* Mock d'ObjectPasswords pour simuler la BDD
        var userId = Guid.NewGuid();

        var objectPasswords = new[]
        {
            new ObjectPassword
            {
                Id = Guid.NewGuid(),
                Site = "Site1",
                Username = "Toto",
                Password = "Toto",
                Category = "Category1",
                CreatedAt = DateTime.Now,
                AccountId = userId
            },
            new ObjectPassword
            {
                Id = Guid.NewGuid(),
                Site = "Site2",
                Username = "Tata",
                Password = "Tata",
                Category = "Category2",
                CreatedAt = DateTime.Now,
                AccountId = userId
            }
        };
        _sqliteDbContext.ObjectPasswords.AddRange(objectPasswords);
        await _sqliteDbContext.SaveChangesAsync();
        
        //* Mock de la valeur d'entrée
        var token = JwtService.JwtCreateToken(new AccountDTO
        {
            Id = Guid.NewGuid(),
            Username = "Toto",
            CreatedAt = DateTime.Now
        });
        
        //! Act
        var result = _dataRepository.GetPasswords(token).Result;
        
        //! Assert
        Assert.Null(result);
    }
}