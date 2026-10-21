using BankApp.Enums;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class AuthenticationController(AuthenticationService service, AuthenticationView view)
{
    private readonly AuthenticationService _service = service;
    private readonly AuthenticationView _view = view;

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

                case "2": // TODO Register
                    return null;

                case "0":
                    return null;

                default:
                    _view.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    continue;
            }
        }
    }
    private User? Login()
    {
        while (true)
        {
            var username = _view.ReadUsername();
            var password = _view.ReadUserPassword();

            User? user = _service.AuthenticateUser(username, password);

            if (user is null)
            {
                _view.DisplayErrorMessage(ErrorMessages.UserNotFound);
            }
            else if (user.IsLocked)
            {
                _view.DisplayErrorMessage(ErrorMessages.LockedAccount);
                return HandleLoginMenu(user);
            }
            else
            {
                return user;
            }

            var choice = _view.DisplayBackToMainMenu(null);

            switch (choice)
            {
                case "1":
                    continue;
                case "0":
                    return null;
                default:
                    _view.DisplayErrorMessage(ErrorMessages.InvalidOption);

                    while (choice != "0" && choice != "1")
                    {
                        choice = _view.DisplayBackToMainMenu(null);
                    }

                    break;
            }
        }
    }

    private User? HandleLoginMenu(User user)
    {
        while (true)
        {
            var choice = _view.DisplayBackToMainMenu(user);

            switch (choice)
            {
                case "0":
                    // Shows the main menu again
                    return null;
                default:
                    _view.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    break;
            }

        }
    }
}
