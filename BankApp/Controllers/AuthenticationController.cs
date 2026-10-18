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
                        // TODO Display Error Message
                    }
                }
                return user;

            case "2": // TODO Register
                return null;

            case "0":
                return null;

            default: 
                // TODO Display Error for menu choice or refactor solution
                return null;
        }
    }
}
