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

    internal Customer? RegisterCustomer(string username, string password)
    {
        var user = _repository.GetByUsername(username);

        if (user is null)
        {
            if (username.Length < 5)
            {
                // TODO: signal "username too short" to AuthenticationController
                return null;
            }

            if (password.Length < 7)
            {
                // TODO: signal "password too short" to AuthenticationController
                return null;
            }

            // TODO: signal "registration success" to AuthenticationController
            user = _repository.AddUser(username, password);
            return user;
        }

        // TODO: signal "user already exists" to AuthenticationController
        return null;
    }
}
