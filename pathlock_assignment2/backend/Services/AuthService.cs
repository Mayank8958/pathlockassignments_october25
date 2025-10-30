using ProjectManager.Api.Auth;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;

namespace ProjectManager.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _users.GetByUsernameAsync(request.Username);
        if (existing != null) throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        var token = _jwt.GenerateToken(user.Id, user.Username);
        return new AuthResponse { Token = token, Username = user.Username };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByUsernameAsync(request.Username) ?? throw new UnauthorizedAccessException("Invalid credentials");
        if (!_hasher.Verify(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials");
        var token = _jwt.GenerateToken(user.Id, user.Username);
        return new AuthResponse { Token = token, Username = user.Username };
    }
}


