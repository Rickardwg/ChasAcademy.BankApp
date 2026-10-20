using BankApp.Models;

namespace BankApp.Repositories;

internal class UserRepository
{
    private readonly List<User> _users =
    [
        new User
        { 
            Id = Guid.NewGuid(), 
            UserName = "testtest", 
            Password = "testtest", 
            IsAdmin = false
        },
        new User
        {
            Id = Guid.NewGuid(),
            UserName = "adminadmin",
            Password = "adminadmin",
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
