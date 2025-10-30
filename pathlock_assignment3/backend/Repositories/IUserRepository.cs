using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Models;

namespace ProjectManager.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) { _db = db; }

    public Task<User?> GetByUsernameAsync(string username) => _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    public Task<User?> GetByIdAsync(int id) => _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    public async Task AddAsync(User user) { await _db.Users.AddAsync(user); }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}


