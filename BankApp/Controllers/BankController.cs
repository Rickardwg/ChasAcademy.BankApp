using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class BankController(BankService service, BankView view)
{
    private readonly BankService _service = service;
    private readonly BankView _view = view;

    internal void Run(User user)
    {
        // TODO Display WelcomeMessage
        // TODO Display BankMenu
        // TODO Read MenuChoice

        // TODO Handle MenuChoice
        // TODO View Account/Transaction History
        // TODO Transfer Money Menu
        // TODO Loan Menu
        // TODO Add New Account Menu
        // TODO Exit
        // TODO Handle invalid MenuChoice
    }
}