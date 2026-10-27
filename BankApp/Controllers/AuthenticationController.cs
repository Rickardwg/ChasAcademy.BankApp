using BankApp.Common.Results;
using BankApp.Enums;
using BankApp.Enums.Errors;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;
using System.Security.AccessControl;

namespace BankApp.Controllers;

internal class AuthenticationController(AuthenticationService service, AuthenticationView view, ErrorMessageView errorMessageView)
{
    private readonly AuthenticationService _service = service;
    private readonly AuthenticationView _view = view;
    private readonly ErrorMessageView _errorMessageView = errorMessageView;

    internal User? Run() // return null to exit
    {
        _view.DisplayWelcomeMessage();

        while (true)
        {
            var menuChoice = _view.DisplayMenu();

            switch (menuChoice)
            {
                case "1": // Log in
                {
                    User? user = Login();

                    if (user is not null)
                    {
                        return user;
                    }

                    break;
                }

                case "2": // Register
                {
                    User? user = Register();

                    if (user is not null)
                    {
                        return user;
                    }

                    break;
                }

                case "0":
                    return null;

                default:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    continue;
            }
        }
    }

    /// <summary>
    /// Prompts the user for their username and password and attempts to authenticate them.
    /// Displays an error message if the credentials are invalid or the account is locked.
    /// Allows the user to retry login or return to the main menu.
    /// </summary>
    /// <returns>The authenticated user, or null if the user returns to the main menu.</returns>
    private User? Login()
    {
        while (true)
        {
            var username = _view.ReadUsername();
            var password = _view.ReadPassword();

            Result<User, LoginError> loginResult = _service.AuthenticateUser(username, password);

            if (loginResult.IsSuccess)
            {
                return loginResult.Value;
            }

            switch (loginResult.Error)
            {
                case LoginError.InvalidCredentials:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.UserNotFound);
                    break;

                case LoginError.AccountLocked:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.LockedAccount);
                    return HandleLoginMenu();

                default:
                    throw new InvalidOperationException("Unexpected login failure.");
            }

            var choice = _view.DisplayBackToMainMenu(allowRetry: true);

            switch (choice)
            {
                case "1":
                    continue;
                case "0":
                    return null;
                default:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);

                    while (choice != "0" && choice != "1")
                    {
                        choice = _view.DisplayBackToMainMenu(allowRetry: true);
                    }

                    if (choice == "0")
                    {
                        return null;
                    }

                    break;
            }
        }
    }

    private User? Register()
    {
        var customerData = _view.DisplayRegisterMenu();

        while (true)
        {
            var registrationResult = _service.RegisterCustomer(customerData);

            if (registrationResult.IsSuccess)
            {
                return registrationResult.Value;
            }

            RegistrationError error = registrationResult.Error ?? throw new InvalidOperationException("Registration failed without an error.");
            _view.DisplayRegistrationError(error);

            string choice = _view.RetryRegistrationPrompt();

            if (choice == "n")
            {
                return null;
            }

            switch (error)
            {
                case RegistrationError.UsernameAlreadyExists:
                case RegistrationError.InvalidUsername:
                    customerData.UserName = _view.ReadUsername();
                    break;

                case RegistrationError.InvalidFirstName:
                    customerData.FirstName = _view.ReadFirstName();
                    break;

                case RegistrationError.InvalidLastName:
                    customerData.LastName = _view.ReadLastName();
                    break;

                case RegistrationError.InvalidEmailAddress:
                    customerData.EmailAddress = _view.ReadEmailAddress();
                    break;

                case RegistrationError.InvalidPhoneNumber:
                    customerData.PhoneNumber = _view.ReadPhoneNumber();
                    break;

                case RegistrationError.InvalidPassword:
                    customerData.Password = _view.ReadPassword();
                    break;

                default:
                    throw new InvalidOperationException("Unexpected registration failure.");
            }
        }
    }

    /// <summary>
    /// Displays the login menu for a locked user and prompts them to return to the main menu.
    /// Repeats the menu if the user enters an invalid option.
    /// </summary>
    /// <returns>Null to return to the main menu.</returns>
    private User? HandleLoginMenu()
    {
        while (true)
        {
            var choice = _view.DisplayBackToMainMenu(allowRetry: false);

            switch (choice)
            {
                case "0":
                    // Shows the main menu again
                    return null;
                default:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    break;
            }
        }
    }
}
