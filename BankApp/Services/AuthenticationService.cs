using BankApp.Common.Results;
using BankApp.Enums.Errors;
using BankApp.Models;
using BankApp.Repositories;
using System.Text.RegularExpressions;

namespace BankApp.Services;

internal class AuthenticationService(UserRepository repository)
{
    private readonly UserRepository _repository = repository;

    internal Result<User, LoginError> AuthenticateUser(string username, string password)
    {
        var user = _repository.GetByUsername(username);

        if (user is null)
        {
            return Result<User, LoginError>.Failure(LoginError.InvalidCredentials);
        }

        if (user.IsLocked)
        {
            return Result<User, LoginError>.Failure(LoginError.AccountLocked);
        }

        if (user.Password != password)
        {
            user.FailedLoginCounter++;

            if (user.FailedLoginCounter >= 3)
            {
                user.IsLocked = true;
                return Result<User, LoginError>.Failure(LoginError.AccountLocked);
            }

            return Result<User, LoginError>.Failure(LoginError.InvalidCredentials);
        }

        user.FailedLoginCounter = 0; // Reset counter on successful login
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
            Password = customerData.Password
        };

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
