using BankApp.Enums;
using BankApp.Models;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class BankController(BankService service, BankView view, AccountRepository repository, ErrorMessageView errorMessageView, TransactionSchedulingService scheduler)
{
    private readonly AccountRepository _repository = repository;
    private readonly BankService _service = service;
    private readonly BankView _view = view;
    private readonly ErrorMessageView _errorMessageView = errorMessageView;
    private readonly TransactionSchedulingService _scheduler = scheduler;
    internal void Run(Customer user)
    {
        _service.SetActiveUser(user);

        while (true)
        {
            ConsoleHeader.DisplayLogo();
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

                    _view.WaitForContinue();
                    break;

                case ("2"): // Transfer
                    List<BankAccount>? ownAccountsForTransfer = _repository.GetBankAccountsOwnedByCustomer(_service.ActiveUser);

                    if (ownAccountsForTransfer is null)
                    {
                        _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                        _view.WaitForContinue();
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
                                _errorMessageView.DisplayErrorMessage(ErrorMessages.NoOtherCustomerAccounts);
                                _view.WaitForContinue();
                                break;
                            }

                            List<PersonalAccount> otherPersonalAccounts = otherAccountsForTransfer.OfType<PersonalAccount>().ToList();

                            HandleOtherCustomerTransfer(ownAccountsForTransfer, otherPersonalAccounts);
                            break;

                        case "0": // Go back to main menu
                            break;

                        default:
                            _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                            _view.WaitForContinue();
                            break;
                    }

                    break;

                case ("3"):
                    List<BankAccount>? accountsForLoan = _repository.GetBankAccountsOwnedByCustomer(user);
                    if (accountsForLoan is not null)
                    {
                        LoanAccount? loanAccount
                         = accountsForLoan.OfType<LoanAccount>().FirstOrDefault();

                        if (loanAccount is not null)
                        {
                            HandleBorrowMoney(loanAccount);
                        }
                        else
                        {
                            _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                            _view.WaitForContinue();
                        }
                    }
                    else
                    {
                        _errorMessageView.DisplayErrorMessage(ErrorMessages.NoBankAccountFound);
                        _view.WaitForContinue();
                    }
                    break;

                case ("4"): // Create Account
                    while (true)
                    {
                        var accountTypeChoice = _view.ReadEnumChoice<AccountType>("Account types");

                        if (accountTypeChoice is not AccountType accountType)
                        {
                            break;
                        }

                        var currencyChoice = _view.ReadEnumChoice<Currency>("Available currencies");

                        if (currencyChoice is not Currency currency)
                        {
                            continue; // Go back to account type selection
                        }

                        _service.CreateNewAccount(accountType, currency);
                        break;
                    }

                    break;

                case ("0"): // Logout
                    return;

                default: // Inaccessible
                    _errorMessageView.DisplayErrorMessage(ErrorMessages.InvalidOption);
                    _view.WaitForContinue();
                    break;
            }
        }
    }

    private void HandleSelfTransfer(List<BankAccount> accounts)
    {
        if (accounts.Count < 2)
        {
            // TODO: Display error message via ErrorMessageView
            Console.WriteLine("Self transfers require two accounts.");
            _view.WaitForContinue();
            return;
        }

        int sourceIndex = _view.SelectOwnAccount(accounts, "Select source account:");
        int destinationIndex = _view.SelectOwnAccount(accounts, "Select destination account:");

        if (sourceIndex == destinationIndex)
        {
            _errorMessageView.DisplayErrorMessage(ErrorMessages.SameSourceAndDestinationAccount);
            _view.WaitForContinue();
            return;
        }    

        BankAccount sourceAccount = accounts[sourceIndex];
        BankAccount destinationAccount = accounts[destinationIndex];

        decimal amount = _view.GetTransferAmount();


        PendingTransaction transaction = new(sourceAccount, destinationAccount, amount);
        _scheduler.EnqueueTransaction(transaction);
        _view.DisplayTransferScheduled();

        _view.WaitForContinue();
    }

    private void HandleOtherCustomerTransfer(List<BankAccount> ownAccounts, List<PersonalAccount> otherAccounts)
    {
        List<BankAccount> nonLoanAccounts = [];

        foreach (BankAccount bankAccount in ownAccounts)
        {
            if (bankAccount is not LoanAccount)
            {
                nonLoanAccounts.Add(bankAccount);
            }
        }

        if (nonLoanAccounts.Count < 1)
        {
            // TODO: Display error message via ErrorMessageView
            Console.WriteLine("You need at least one non-loan account to transfer to another customer");
            _view.WaitForContinue();
            return;
        }

        if (otherAccounts.Count < 1)
        {
            // TODO: Display error message via ErrorMessageView
            Console.WriteLine("No other customer has an account you can send money to.");
            _view.WaitForContinue();
            return;
        }

        int sourceIndex = _view.SelectOwnAccount(nonLoanAccounts, "Select source account:");
        int destinationIndex = _view.SelectOtherCustomerAccount(otherAccounts, "Select destination account:");

        BankAccount sourceAccount = nonLoanAccounts[sourceIndex];
        PersonalAccount destinationAccount = otherAccounts[destinationIndex];

        // Own and others’ accounts never overlap

        decimal amount = _view.GetTransferAmount();

        PendingTransaction transaction = new(sourceAccount, destinationAccount, amount);
        _scheduler.EnqueueTransaction(transaction);
        _view.DisplayTransferScheduled();

        _view.WaitForContinue();
    }

    private void HandleInterestPreview(SavingsAccount savingsAccount)
    {
        decimal depositAmount = _view.GetDepositAmount();
        int years = _view.GetInterestYears();

        if (depositAmount <= 0 || years <= 0)
        {
            _view.DisplayInvalidInterestInput();
            _view.WaitForContinue();
            return;
        }

        decimal expectedInterest = _service.CalculateExpectedInterest(savingsAccount, depositAmount, years);

        _view.DisplayInterestPreview
            (depositAmount, savingsAccount.InterestRate, years, expectedInterest);
        _view.WaitForContinue();
    }

    public void HandleBorrowMoney(LoanAccount loanAccount)
    {
        decimal loanAmount = _view.GetLoanAmount();
        int years = _view.GetInterestYears();

        if (loanAmount <= 0 || years <= 0 || loanAccount.InterestRate <= 0)
        {
            _view.DisplayInvalidLoanInput();
            _view.WaitForContinue();
            return;
        }

        decimal totalInterest = _service.CalculateLoanInterest(loanAccount, loanAmount, years);

        _view.DisplayLoanPreview(loanAmount, loanAccount.InterestRate, years, totalInterest);

        bool isConfirmed = _view.ConfirmLoan();

        if (!isConfirmed)
        {
            return;
        }

        bool loanSuccess = _service.BorrowMoney(loanAccount, loanAmount, years);

        _view.DisplayLoanResult(loanSuccess);
        _view.WaitForContinue();
    }
}