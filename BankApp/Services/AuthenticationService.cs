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
            user.FailedLoginCounter++;

            if (user.FailedLoginCounter >= 3)
            {
                user.IsLocked = true;
                return user;
            }

            return null;
        }

        user.FailedLoginCounter = 0; // Reset counter on successful login
        return user;
    }

    internal User? RegisterUser(string username, string password)
    {
        var user = _repository.GetByUsername(username);

        if (user is null)
        {
            if (username.Length <= 5)
            {
                // TODO show error message
                return null;
            }

            if (password.Length == 0)
            {
                // TODO show error message
                return null;
            }

            // TODO show success message
            _repository.AddUser(username, password);
            return user;
        }

        return null;
    }
}
