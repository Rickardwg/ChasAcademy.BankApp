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

    internal User AddUser(string username, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Password = password,
            IsAdmin = false
        };

        _users.Add(user);

        return user;
    }
}
