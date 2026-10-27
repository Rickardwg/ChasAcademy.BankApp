using BankApp.Common.Results;
using BankApp.Enums;
using BankApp.Enums.Errors;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

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
                    User? user = Login();

                    if (user is not null)
                    {
                        return user;
                    }

                    break;

                case "2": // Register
                    var customerData = _view.DisplayRegisterMenu();

                    var registrationResult = _service.RegisterCustomer(customerData);

                    if (registrationResult.IsSuccess)
                    {
                        return registrationResult.Value;
                    }

                    switch (registrationResult.Error)
                    {
                        case RegistrationError.UsernameAlreadyExists:
                            _view.DisplayRegistrationError(RegistrationError.UsernameAlreadyExists);
                            break;

                        case RegistrationError.InvalidFirstName:
                            _view.DisplayRegistrationError(RegistrationError.InvalidFirstName);
                            break;

                        case RegistrationError.InvalidLastName:
                            _view.DisplayRegistrationError(RegistrationError.InvalidLastName);
                            break;

                        case RegistrationError.InvalidEmailAddress:
                            _view.DisplayRegistrationError(RegistrationError.InvalidEmailAddress);
                            break;

                        case RegistrationError.InvalidPhoneNumber:
                            _view.DisplayRegistrationError(RegistrationError.InvalidPhoneNumber);
                            break;

                        case RegistrationError.InvalidUsername:
                            _view.DisplayRegistrationError(RegistrationError.InvalidUsername);
                            break;

                        case RegistrationError.InvalidPassword:
                            _view.DisplayRegistrationError(RegistrationError.InvalidPassword);
                            break;

                        default:
                            throw new InvalidOperationException("Unexpected registration failure.");
                    }
                    break;

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
            var password = _view.ReadUserPassword();

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
