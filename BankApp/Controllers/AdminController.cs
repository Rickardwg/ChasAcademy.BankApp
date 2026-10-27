using BankApp.Enums;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class AdminController(AdminView adminView, AuthenticationView authenticationView, AuthenticationService authenticationService, ErrorMessageView errorMessageView)
{
    private readonly AdminView _view = adminView;
    private readonly AuthenticationView _authView = authenticationView;
    private readonly AuthenticationService _authService = authenticationService;
    private readonly ErrorMessageView _errorMessageView = errorMessageView;

    internal void Run(User user)
    {
        _view.DisplayWelcomeMessage(user);

        while (true)
        {
            // Main menu
            var mainMenuChoice = _view.DisplayMenu();

            switch (mainMenuChoice)
            {
                case "1": // Manage users
                    ShowUserManagementMenu();

                    break;
                case "2": // Manage interest
                    break;
                case "3": // Manage currency
                    break;
                case "0": // Logout
                    return;
                default:
                    break;
            }
        }
    }

    private void ShowUserManagementMenu()
    {
        bool showMenu = true;

        while (showMenu)
        {
            var userToManageChoice = _view.DisplayUserManagementMenu();

            switch (userToManageChoice)
            {
                case "1": // Add
                    bool returnToMainMenu = ShowUserTypeMenu();

                    if (returnToMainMenu)
                    {
                        showMenu = false;
                    }

                    break;
                case "2":
                    // Change
                    break;
                case "3":
                    // Remove
                    break;
                case "0":
                    // Return to Main Menu
                    showMenu = false;
                    break;
                default:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    break;
            }
        }
    }

    private bool ShowUserTypeMenu()
    {
        while (true)
        {

            var userTypeChoice = _view.DisplayUserTypeMenu();

            switch (userTypeChoice)
            {
                case "1":
                    // Admin

                    return true; // Return to main menu
                case "2":
                    // Customer
                    var customerData = _authView.DisplayRegisterMenu();
                    _authService.RegisterCustomer(customerData);

                    return true; // Return to main menu
                case "9": // Go back

                    return false; // Shows the previous menu
                case "0":

                    return true; // Return to main menu
                default:
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    return false; // Shows the previous menu
            }
        }
    }
}
