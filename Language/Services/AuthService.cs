using Language.Database;
using Language.Model;
using Language.Requests;
using Microsoft.EntityFrameworkCore;

namespace Language.Services;

public class AuthService
{
    public readonly MainDbContext _dbContext;

    public AuthService(MainDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }

    public async Task<User> RegisterUser(RegisterUser user)
    {
        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = user.Name,
            Email = user.Email,
            Password = user.Password
        };
        
        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        return newUser;
    }
}