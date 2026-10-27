using BankApp.Enums;
using BankApp.Models;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class BankController(BankService service, BankView view, AccountRepository repository, ErrorMessageView errorMessageView)
{
    private readonly AccountRepository _repository = repository;
    private readonly BankService _service = service;
    private readonly BankView _view = view;
    private readonly ErrorMessageView _errorMessageView = errorMessageView;

    internal void Run(Customer user)
    {
        _service.SetActiveUser(user);


        while (true)
        {
            var menuChoice = _view.DisplayMenu();

            switch (menuChoice)
            {
                case ("1"): // Accounts
                    List<BankAccount>? accountsToDisplay = _repository.GetBankAccounts();

                    if (accountsToDisplay is not null)
                    {
                        _view.DisplayAccounts(accountsToDisplay);
                    }
                    else
                    {
                        _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                    }
                    break;

                case ("2"): // Transfer
                    List<BankAccount>? accountsForTransfer = _repository.GetBankAccounts();

                    if (accountsForTransfer is not null)
                    {
                        List<PersonalAccount> personalAccounts = accountsForTransfer.OfType<PersonalAccount>().ToList();
                        HandleTransfer(personalAccounts);
                    }
                    else
                    {
                        _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                    }

                    break;

                case ("3"): // TODO Loan
                    break;

                case ("4"): // Create Account
                    var accountTypeChoice = _view.ReadEnumChoice<AccountType>("Account types");

                    if (accountTypeChoice is not AccountType accountType)
                    {
                        break;
                    }

                    var currencyChoice = _view.ReadEnumChoice<Currency>("Available currencies");

                    if (currencyChoice is not Currency currency)
                    {
                        break;
                    }

                    _service.CreateNewAccount(accountType, currency);
                    break;

                case ("0"): // Logout
                    return;

                default: // Inaccessible
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    break;
            }
        }
    }

    private void HandleTransfer(List<PersonalAccount> accounts)
    {
        if (accounts.Count < 2)
        {
            _errorMessageView.DisplayErrorMessage(ErrorMessages.NotEnoughAccounts);
            return;
        }

        int sourceIndex = _view.SelectAccount(accounts, "Select source account:");
        int destinationIndex = _view.SelectAccount(accounts, "Select destination account:");

        if (sourceIndex < 0 || destinationIndex < 0 ||
            sourceIndex >= accounts.Count || destinationIndex >= accounts.Count)
        {
            return;
        }

        if (sourceIndex == destinationIndex)
        {
            _errorMessageView.DisplayErrorMessage(ErrorMessages.SameSourceAndDestinationAccount);
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

        if (depositAmount <= 0 || years <= 0)
        {
            _view.DisplayInvalidInterestInput();
            return;
        }

        decimal expectedInterest = _service.CalculateExpectedInterest(savingsAccount, depositAmount, years);

        _view.DisplayInterestPreview
            (depositAmount, savingsAccount.InterestRate, years, expectedInterest);
    }
}