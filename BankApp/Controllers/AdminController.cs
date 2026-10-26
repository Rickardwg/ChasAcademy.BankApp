using BankApp.Enums;
using BankApp.Models;
using BankApp.Views;

namespace BankApp.Controllers;

public class AdminController(AdminView adminView, ErrorMessageView errorMessageView)
{
    private readonly AdminView _view = adminView;
    private readonly ErrorMessageView _errorMessageView = errorMessageView;

    internal void Run(User user)
    {
        _view.DisplayWelcomeMessage(user);

        while (true)
        {
            var mainMenuChoice = _view.DisplayMenu();

            switch (mainMenuChoice)
            {
                case "1":
                    bool showEmployeeMenu = true;

                    while (showEmployeeMenu)
                    {
                        var employeeMenuChoice = _view.DisplayManagementMenu("employee");

                        switch (employeeMenuChoice)
                        {
                            case "1":
                                // TODO add new employee
                                break;
                            case "2":
                                // TODO remove employee
                                break;
                            case "3":
                                // TODO show employees
                                break;
                            case "0":
                                // Return to Main Menu
                                showEmployeeMenu = false;
                                break;
                            default:
                                _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                                break;
                        }
                    }

                    break;
                case "2":
                    bool showCustomerMenu = true;

                    while (showCustomerMenu)
                    {
                        var customerMenuChoice = _view.DisplayManagementMenu("customer");

                        switch (customerMenuChoice)
                        {
                            case "1":
                                // TODO add new customer
                                break;
                            case "2":
                                // TODO remove customer
                                break;
                            case "3":
                                // TODO show customers
                                break;
                            case "0":
                                // Return to Main Menu
                                showCustomerMenu = false;
                                break;
                            default:
                                _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                                break;
                        }
                    }
                    break;
                case "0":
                    break;
                default:
                    break;
            }
        }
    }
}
