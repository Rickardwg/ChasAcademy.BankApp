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

    private void HandleTransfer(List<PersonalAccount> accounts)
    {
        int sourceIndex = _view.SelectAccount(accounts);
        int destinationIndex = _view.SelectAccount(accounts);

        if (sourceIndex < 0 ||
            destinationIndex < 0 ||
            sourceIndex >= accounts.Count ||
            destinationIndex >= accounts.Count)
        {
            return;
        }

        PersonalAccount sourceAccount = accounts[sourceIndex];
        PersonalAccount destinationAccount = accounts[destinationIndex];

        decimal amount = _view.GetTransferAmount();

        bool transferSuccess = _service.TransferMoney(sourceAccount, destinationAccount, amount);

        _view.DisplayTransferResult(transferSuccess);
    }
}