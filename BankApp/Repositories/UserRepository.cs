using BankApp.Models;

namespace BankApp.Repositories;

internal class UserRepository
{
    private readonly List<User> _users =
    [
        new User
        { 
            Id = Guid.NewGuid(), 
            UserName = "test", 
            Password = "test", 
            IsAdmin = false
        },
        new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Password = "admin",
            IsAdmin = true
        }
    ];

    internal User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.UserName == username);
    }

    internal void AddUser(string username, string password)
    {
        new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Password = password,
            IsAdmin = false
        };
    }
}
