using BankApp.Models;
using BankApp.Repositories;

namespace BankApp.Services;

internal class AuthenticationService(UserRepository repository)
{
    private readonly UserRepository _repository = repository;

    internal User? AuthenticateUser(string username, string password)
    {
        var user = _repository.GetByUsername(username);

        if (user is null)
        {
            return null;
        }

        if (user.IsLocked)
        {
            return user;
        }

        if (user.Password != password)
        {
            return null;
        }

        return user;
    }
}
