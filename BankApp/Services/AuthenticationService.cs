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

    internal Result<Customer, RegistrationError> RegisterCustomer((string FirstName, string LastName, string EmailAddress, string PhoneNumber, string UserName, string Password) customerData)
    {
        var existingCustomer = _repository.GetByUsername(customerData.UserName);

        if (existingCustomer is not null)
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.UsernameAlreadyExists);
        }

        if (customerData.FirstName.Length < 2)
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidFirstName);
        }

        if (customerData.LastName.Length < 4)
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidLastName);
        }

        if (!IsValidEmailAddress(customerData.EmailAddress))
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidEmailAddress);
        }

        if (!IsValidPhoneNumber(customerData.PhoneNumber))
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidPhoneNumber);
        }

        if (customerData.UserName.Length < 5)
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidUsername);
        }

        if (customerData.Password.Length < 7)
        {
            return Result<Customer, RegistrationError>.Failure(RegistrationError.InvalidPassword);
        }
            
        Customer customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = customerData.FirstName,
            LastName = customerData.LastName,
            EmailAddress = customerData.EmailAddress,
            PhoneNumber = customerData.PhoneNumber,
            UserName = customerData.UserName,
        };

        customer.PasswordHash = _passwordHasher.HashPassword(customer, customerData.Password); // user-object is needed for hashing, customer has to be created first.

        _repository.AddUser(customer);
        return Result<Customer, RegistrationError>.Success(customer);
    }

    // Validates email address
    private bool IsValidEmailAddress(string emailAddress)
    {
        return Regex.IsMatch(emailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    // Validates a Swedish phone number
    private bool IsValidPhoneNumber(string phoneNumber)
    {
        phoneNumber = phoneNumber.Replace("-", "");
        phoneNumber = phoneNumber.Replace(" ", "");

        if (phoneNumber.StartsWith("+46"))
        {
            phoneNumber = phoneNumber.Replace("+46", "0");
        }

        bool isValid = Regex.IsMatch(phoneNumber, @"^07\d{8}$");

        return isValid;
    }
}
