using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManager.Api.Auth;
using ProjectManager.Api.Data;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Repositories;
using ProjectManager.Api.Services;
using Xunit;

public class AuthServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Register_Then_Login_Works()
    {
        using var db = CreateDb();
        var users = new UserRepository(db);
        var hasher = new BcryptPasswordHasher();
        var jwt = new JwtService(Options.Create(new JwtSettings { Secret = new string('x', 64), Issuer = "t", Audience = "t" }));
        var svc = new AuthService(users, hasher, jwt);

        var reg = await svc.RegisterAsync(new RegisterRequest { Username = "alice", Password = "password123" });
        Assert.False(string.IsNullOrWhiteSpace(reg.Token));

        var login = await svc.LoginAsync(new LoginRequest { Username = "alice", Password = "password123" });
        Assert.False(string.IsNullOrWhiteSpace(login.Token));
    }
}


