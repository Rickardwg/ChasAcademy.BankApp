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
        var menuChoice = _view.DisplayMenu();

        switch (menuChoice)
        {
            case "1": // Log in
                User? user = null;

                while (user is null)
                {
                    var username = _view.ReadUsername();
                    var password = _view.ReadUserPassword();

                    user = _service.AuthenticateUser(username, password);

                    if (user is null)
                    {
                        _view.DisplayErrorMessage(ErrorMessages.UserNotFound);
                    }
                    else if (user.IsLocked)
                    {
                        _view.DisplayErrorMessage(ErrorMessages.LockedAccount);
                        return null;
                    }

                }
                return user;

            case "2": // Register
                {
                    var username = _view.ReadUsername();
                    var password = _view.ReadUserPassword();

                    user = _service.RegisterUser(username, password);

                    if (user is null)
                    {
                        // TODO: Display Error for each of these outcomes:
                        // Too short username
                        // Too short password
                        // User already exist

                        return null;
                    }

                    // TODO: Display registration succeeded confirmation
                    return user;
                }
            case "0":
                return null;

            default:
                return null;
        }
    }
}
