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
                    List<BankAccount>? accountsToDisplay = _repository.GetBankAccountsOwnedByCustomer(_service.ActiveUser);

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
                    List<BankAccount>? ownAccountsForTransfer = _repository.GetBankAccountsOwnedByCustomer(_service.ActiveUser);

                    if (ownAccountsForTransfer is null)
                    {
                        _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                        break;
                    }

                    menuChoice = _view.DisplayTransferMoneyMenu();

                    switch (menuChoice)
                    {
                        case "1": // Transfer between own accounts

                            HandleSelfTransfer(ownAccountsForTransfer);
                            break;

                        case "2": // Transfer to another customer's account

                            List<BankAccount>? otherAccountsForTransfer = _repository.GetBankAccountsOwnedByOthers(_service.ActiveUser);

                            if (otherAccountsForTransfer is null)
                            {
                                // TODO: add error message that no other customers have accounts in the bank
                                break;
                            }

                            List<PersonalAccount> otherPersonalAccounts = otherAccountsForTransfer.OfType<PersonalAccount>().ToList();

                            HandleOtherCustomerTransfer(ownAccountsForTransfer, otherPersonalAccounts);
                            break;

                        case "0": // Go back to main menu
                            break;

                        default:
                            _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                            break;
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

                case ("0"): // Exit
                    return;

                default: // Inaccessible
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    break;
            }
        }
    }

    private void HandleSelfTransfer(List<BankAccount> accounts)
    {
        List<BankAccount> nonLoanAccounts = [];

        foreach (BankAccount bankAccount in accounts)
        {
            if (bankAccount is not LoanAccount)
            {
                nonLoanAccounts.Add(bankAccount);
            }
        }

        if (nonLoanAccounts.Count < 1 || accounts.Count < 2)
        {
            // TODO: add error message:
            Console.WriteLine("Self transfers require two accounts, at least one of which is not a loan account."); // TEMP
            return;
        }

        int sourceIndex = _view.SelectOwnAccount(nonLoanAccounts, "Select source account:");
        int destinationIndex = _view.SelectOwnAccount(accounts, "Select destination account:");

        if (sourceIndex < 0 || destinationIndex < 0 ||
            sourceIndex >= nonLoanAccounts.Count || destinationIndex >= accounts.Count)
        {
            return;
        }

        BankAccount sourceAccount = nonLoanAccounts[sourceIndex];
        BankAccount destinationAccount = accounts[destinationIndex];

        if (sourceAccount.AccountNumber == destinationAccount.AccountNumber)
        {
            _errorMessageView.DisplayErrorMessage(ErrorMessages.SameSourceAndDestinationAccount);
            return;
        }

        decimal amount = _view.GetTransferAmount();

        bool transferSuccess = _service.TransferMoney(sourceAccount, destinationAccount, amount);

        _view.DisplayTransferResult(transferSuccess);
    }

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