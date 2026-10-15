using BankApp.Models;

namespace BankApp.Repositories;

internal class UserRepository
{
    private readonly List<User> _users = [];

    internal User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.UserName == username);
    }
}
