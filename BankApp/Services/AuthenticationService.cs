using BankApp.Common.Results;
using BankApp.Enums.Errors;
using BankApp.Models;
using BankApp.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BankApp.Services;

internal class AuthenticationService(UserRepository repository, IPasswordHasher<User> passwordHasher)
{
    private readonly UserRepository _repository = repository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private const int MaxFailedLoginAttemps = 3;
    private int _failedLoginAttempts = 0;

    internal Result<User, LoginError> AuthenticateUser(string username, string password)
    {
        if (_failedLoginAttempts == MaxFailedLoginAttemps)
        {
            return Result<User, LoginError>.Failure(LoginError.LoginLimitReached);
        }

        var user = _repository.GetByUsername(username);

        if (user is null)
        {
            _failedLoginAttempts++;

            if (_failedLoginAttempts == MaxFailedLoginAttemps)
            {
                return Result<User, LoginError>.Failure(LoginError.LoginLimitReached);
            }

            return Result<User, LoginError>.Failure(LoginError.InvalidCredentials);
        }

        PasswordVerificationResult verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _failedLoginAttempts++;

            if (_failedLoginAttempts == MaxFailedLoginAttemps)
            {
                return Result<User, LoginError>.Failure(LoginError.LoginLimitReached);
            }

            return Result<User, LoginError>.Failure(LoginError.InvalidCredentials);
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
        }

        _failedLoginAttempts = 0; // Reset counter on successful login
        return Result<User, LoginError>.Success(user);
    }
}
