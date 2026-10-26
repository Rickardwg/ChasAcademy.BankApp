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

                    var customer = _service.RegisterCustomer(customerData);

                    if (customer is null)
                    {
                        // TODO: Display Error for each of these outcomes:
                        // Invalid first name
                        // Invalid last name
                        // Invalid email address
                        // Non-Swedish phone number
                        // Too short username
                        // Too short password
                        // User already exist

                        return null;
                    }

                    // TODO: Display registration succeeded confirmation
                    return customer;

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

                    break;
            }
        }
    }

    /// <summary>
    /// Displays the login menu for a locked user and prompts them to return to the main menu.
    /// Repeats the menu if the user enters an invalid option.
    /// </summary>
    /// <param name="user">The locked user attempting to log in.</param>
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
