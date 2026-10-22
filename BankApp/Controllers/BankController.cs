using BankApp.Enums;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class BankController(BankService service, BankView view)
{
    private readonly BankService _service = service;
    private readonly BankView _view = view;

    internal void Run(Customer user)
    {
        _service.SetActiveUser(user);

        var menuChoice = _view.DisplayMenu();

        switch (menuChoice)
        {
            case ("1"): // TODO Accounts
                break;

            case ("2"): // TODO Transfer
                break;

            case ("3"): // TODO Loan
                break;

            case ("4"): // Create Account
                var accountTypeChoice = _view.ReadAccountTypeChoice();

                if (accountTypeChoice is not AccountType accountType)
                {
                    break;
                }

                var currencyChoice = _view.ReadCurrencyChoice();

                if (currencyChoice is not Currency currency)
                {
                    break;
                }

                _service.CreateNewAccount(accountType, currency);
                break;

            case ("0"): // Exit
                return;

            default: // Inaccessible
                return;
        }
    }

    private void HandleTransfer(List<PersonalAccount> accounts)
    {
        int sourceIndex = _view.SelectAccount(accounts, "Select source account");
        int destinationIndex = _view.SelectAccount(accounts, "Select destination account");

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

    private void HandleInterestPreview(SavingsAccount savingsAccount)
    {
        decimal depositAmount = _view.GetDepositAmount();
        int years = _view.GetInterestYears();
        decimal expectedInterest = _service.CalculateExpectedInterest(savingsAccount, depositAmount, years);

        _view.DisplayInterestPreview
            (depositAmount, savingsAccount.InterestRate, years, expectedInterest);
    }
}