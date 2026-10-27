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
            PasswordHash = "AQAAAAIAAYagAAAAEE4bvuDIujhSBmdHGt9Wp0LaIRXailt7Y/S/JsblpNPFIeSCdMtEm+L/enahIUy8LQ==", // "test"
            IsAdmin = false
        },
        new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            PasswordHash = "AQAAAAIAAYagAAAAEGvWqRz58L2F5Vo0LiptrwDgGGLu8W119w79D6t8MCA5Z4mZgAwGD0BGJDHknWDXIg==", // "admin"
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
