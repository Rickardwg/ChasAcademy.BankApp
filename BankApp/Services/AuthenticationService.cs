using BankApp.Models;
using BankApp.Repositories;
using System.Text.RegularExpressions;

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

    internal Customer? RegisterCustomer((string FirstName, string LastName, string EmailAddress, string PhoneNumber, string UserName, string Password) customerData)
    {
        var existingCustomer = _repository.GetByUsername(customerData.UserName);

        if (existingCustomer is null)
        {
            if (customerData.FirstName.Length < 2)
            {
                // TODO: signal "first name too short" to AuthenticationController
                return null;
            }

            if (customerData.LastName.Length < 4)
            {
                // TODO: signal "last name too short" to AuthenticationController
                return null;
            }

            if (!IsValidEmailAddress(customerData.EmailAddress))
            {
                // TODO: signal "unvalid email address" to AuthenticationController
                return null;
            }

            if (!IsValidPhoneNumber(customerData.PhoneNumber))
            {
                // TODO: signal "non-swedish phone number" to AuthenticationController
                return null;
            }

            if (customerData.UserName.Length < 5)
            {
                // TODO: signal "username too short" to AuthenticationController
                return null;
            }

            if (customerData.Password.Length < 7)
            {
                // TODO: signal "password too short" to AuthenticationController
                return null;
            }
            
            Customer customer = new Customer
            {
                // Not assigned here, so they keep their defaults:
                // IsLocked = false, IsAdmin = false, FailedLoginCounter = 0
                Id = Guid.NewGuid(),
                FirstName = customerData.FirstName,
                LastName = customerData.LastName,
                EmailAddress = customerData.EmailAddress,
                PhoneNumber = customerData.PhoneNumber,
                UserName = customerData.UserName,
                Password = customerData.Password
            };

            _repository.AddUser(customer);
            // TODO: signal "registration success" to AuthenticationController
            return customer;
        }

        // TODO: signal "customer already exists" to AuthenticationController
        return null;
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
