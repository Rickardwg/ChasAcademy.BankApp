using BankApp.Enums;
using BankApp.Enums.Errors;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class AdminController(AdminView adminView, AdminService service, ErrorMessageView errorMessageView)
{
    private readonly AdminView _view = adminView;
    private readonly AdminService _service = service;
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
                    ConsoleHeader.DisplayLogo();
                    string typeChoice = _view.DisplayUserTypeMenu();

                    if (typeChoice == "0")
                    {
                        break;
                    }

                    var customerData = _view.DisplayRegisterMenu();

                    while (true)
                    {
                        var registrationResult = _service.RegisterCustomer(customerData);

                        if (registrationResult.IsSuccess)
                        {
                            _view.DisplayRegistrationSuccess(registrationResult.Value!.UserName);
                            break;
                        }

                        RegistrationError error = registrationResult.Error ?? throw new InvalidOperationException("Registration failed without an error.");

                        ConsoleHeader.DisplayLogo();
                        _view.DisplayRegistrationError(error);

                        string retryChoice = _view.RetryRegistrationPrompt();

                        if (retryChoice == "n")
                        {
                            break;
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
}
