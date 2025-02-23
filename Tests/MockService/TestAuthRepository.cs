using System.Text;
using PassGuard.Api.Database;
using PassGuard.Api.Repositories;

namespace Tests.MockService;

public class TestAuthRepository : AuthRepository
{
    public TestAuthRepository(SqliteDbContext sqliteDbContext) : base(sqliteDbContext)
    {
        
    }
}