using System.Net.Security;
using System.Runtime.CompilerServices;
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

                    // Action menu
                    bool showUserManagementMenu = true;

                    while (showUserManagementMenu)
                    {
                        var userToManageChoice = _view.DisplayUserManagementMenu();

                        switch (userToManageChoice)
                        {
                            case "1": // Add

                                // Choose user menu
                                bool showUserTypeMenu = true;

                                while (showUserTypeMenu)
                                {

                                    var userTypeChoice = _view.DisplayUserTypeMenu();

                                    switch (userTypeChoice)
                                    {
                                        case "1":
                                            // Admin
                                            showUserTypeMenu = false;
                                            showUserManagementMenu = false;
                                            break;
                                        case "2":
                                            // Customer
                                            var customerData = _authView.DisplayRegisterMenu();
                                            _authService.RegisterCustomer(customerData);

                                            showUserTypeMenu = false;
                                            showUserManagementMenu = false;
                                            break;
                                        case "9": // Go back
                                            showUserTypeMenu = false;
                                            break;
                                        case "0": // Return to Main Menu
                                            showUserTypeMenu = false;
                                            showUserManagementMenu = false;
                                            break;
                                        default:
                                            _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                                            break;
                                    }
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
                                showUserManagementMenu = false;
                                break;
                            default:
                                _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                                break;
                        }
                    }

                    break;
                case "2": // Manage interest
                    break;
                case "3": // Manage currency
                    break;
                case "0": // Logout
                    break;
                default:
                    break;
            }
        }
    }
}
