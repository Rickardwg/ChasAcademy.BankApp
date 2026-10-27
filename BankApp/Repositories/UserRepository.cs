using BankApp.Models;

namespace BankApp.Repositories;

internal class UserRepository
{
    private readonly List<User> _users =
    [
        new Customer
        { 
            Id = Guid.NewGuid(), 
            UserName = "test", 
            PasswordHash = "test", 
            IsAdmin = false
        },
        new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = "admin",
            IsAdmin = true
        }
    ];

    internal User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.UserName == username);
    }

    internal void AddUser(User user)
    {
        _users.Add(user);
    }
}
