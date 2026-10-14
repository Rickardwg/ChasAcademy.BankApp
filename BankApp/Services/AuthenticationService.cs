using BankApp.Models;
using BankApp.Repositories;

namespace BankApp.Services;

internal class AuthenticationService(UserRepository repository)
{
    private readonly UserRepository _repository = repository;

    internal User? AuthenticateUser(string username, string password)
    {
        var user = _repository.GetByUsername(username);

        return null;
    }
}
