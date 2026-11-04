using BankApp.Enums;
using BankApp.Models;
using BankApp.Repositories;
using System.Security.Principal;

namespace BankApp.Services;

internal class BankService(AccountRepository repository, ExchangeRateService exchangeRateService)
{
    private readonly AccountRepository _repository = repository;
    private readonly ExchangeRateService _exchangeRateService = exchangeRateService;

    internal Customer ActiveUser { get; private set; }

    internal void SetActiveUser(Customer user)
    {
        ActiveUser = user;
    }

    internal void CreateNewAccount(AccountType type, Currency currency)
    {
        BankAccount account = type switch
        {
            AccountType.Personal => new PersonalAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Personal },
            AccountType.Savings => new SavingsAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Savings },
            AccountType.Loan => new LoanAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Loan },
            _ => throw new InvalidOperationException()
        };

        account.OwnerId = ActiveUser.Id;
        account.Currency = currency;

        _repository.Add(account);
    }

    public bool TransferMoney(BankAccount sourceAccount, BankAccount destinationAccount, decimal amount)
    {
        if (amount <= 0)
        {
            return false;
        }
        
        if (ReferenceEquals(sourceAccount, destinationAccount))
        {
            return false;
        }

        if (!_exchangeRateService.TryGetRate(sourceAccount.Currency, destinationAccount.Currency, out decimal exchangeRate))
        {
            return false;
        }
        
        if (sourceAccount is LoanAccount loanSource)
        {
            // The amount originally borrowed
            decimal principal = loanSource.Owed / (loanSource.InterestRate + 1);

            if (loanSource.Owed <= 0)
            {
                // TODO: Display error message via Result Pattern + ErrorMessageView
                return false;
            }

            if (amount > principal || loanSource.AmountWithdrawn >= principal || amount > (principal - loanSource.AmountWithdrawn))
            {
                // TODO: Display error message via Result Pattern + ErrorMessageView
                Console.WriteLine($"\nWithdrawal is greater than available funds: {principal - loanSource.AmountWithdrawn} {loanSource.Currency}\n");
                return false;
            }

            loanSource.AmountWithdrawn += amount;
        }
        
        if (sourceAccount is PersonalAccount personalSource) // also covers savings account
        {
            if (personalSource.Balance < amount)
            {
                return false;
            }
            
            personalSource.Balance -= amount;
        }

        if (sourceAccount is LoanAccount && destinationAccount is LoanAccount)
        {
            // TODO: Display error message via Result Pattern + ErrorMessageView
            Console.WriteLine("\nBoth accounts can't be loan accounts.\n");
            return false;
        }
        
        decimal destinationAmount = amount * exchangeRate;
        bool hasConverted = sourceAccount.Currency != destinationAccount.Currency;
        
        if (destinationAccount is LoanAccount loanDestination)
        {
            if (destinationAmount > loanDestination.Owed)
            {
                // TODO: Display error message via Result Pattern + ErrorMessageView
                Console.WriteLine($"\nTransfer amount mustn't be greater than the amount owed: {loanDestination.Owed} {loanDestination.Currency}.\n");

                if (sourceAccount is PersonalAccount pS) // Reset the transfer
                {
                    pS.Balance += amount;
                }

                return false;
            }
            
            loanDestination.Owed -= destinationAmount;
            loanDestination.AmountWithdrawn -= destinationAmount;

            if (loanDestination.Owed == 0)
            {
                loanDestination.AmountWithdrawn = 0;
                // TODO: Display success message using Result Pattern
                Console.WriteLine("\nYour loan was successfully paid off!\n");
            }
        }

        if (destinationAccount is PersonalAccount personalDestination)
        {
            personalDestination.Balance += destinationAmount;
        }

        sourceAccount.TransactionHistory.Add(new Transaction
        {
            Type = TransactionType.Transfer,
            Amount = -amount,
            Currency = sourceAccount.Currency,
            ConvertedAmount = hasConverted ? destinationAmount : null,
            ConvertedCurrency = hasConverted ? destinationAccount.Currency : null,
            ExchangeRate = hasConverted ? exchangeRate : null,
            RelatedAccountId = destinationAccount.AccountNumber,
            Desciption = $"Transfer to {destinationAccount.AccountNumber}"
        });

        destinationAccount.TransactionHistory.Add(new Transaction
        {
            Type = TransactionType.Transfer,
            Amount = destinationAmount,
            Currency = destinationAccount.Currency,
            ConvertedAmount = hasConverted ? amount : null,
            ConvertedCurrency = hasConverted ? sourceAccount.Currency : null,
            ExchangeRate = hasConverted ? 1 / exchangeRate : null,
            RelatedAccountId = sourceAccount.AccountNumber,
            Desciption = $"Transfer from {sourceAccount.AccountNumber}"
        });

        return true;
    }

    public decimal CalculateExpectedInterest(SavingsAccount savingsAccount, decimal depositAmount, int years)
    {
        if (depositAmount <= 0 || years <= 0)
        {
            return 0m;
        }

        decimal interestRate = savingsAccount.InterestRate;
        decimal balance = depositAmount;

        for (int year = 1; year <= years; year++)
        {
            balance += balance * interestRate;
        }

        return balance - depositAmount;
    }

    public decimal CalculateLoanInterest(LoanAccount loanAccount, decimal loanAmount, int years)
    {
        if (loanAmount <= 0 || years <= 0 || loanAccount.InterestRate < 0)
        {
            return 0m;
        }

        decimal interestRate = loanAccount.InterestRate;

        return loanAmount * interestRate * years;
    }

    public bool BorrowMoney(LoanAccount loanAccount, decimal loanAmount, int years)
    {
        if (loanAmount <= 0 || years <= 0 || loanAccount.InterestRate < 0)
        {
            return false;
        }

        decimal totalInterest = CalculateLoanInterest(loanAccount, loanAmount, years);

        decimal maxLoanAmount = CalculateMaxLoanAmount(loanAccount);

        decimal newTotalDebt = loanAccount.Owed + loanAmount + totalInterest;

        if (newTotalDebt > maxLoanAmount)
        {
            return false;
        }

        loanAccount.Owed += loanAmount + totalInterest;

        return true;
    }

    public decimal CalculateMaxLoanAmount(LoanAccount loanAccount)
    {
        List<BankAccount>? customerAccounts =
             _repository.GetBankAccountsOwnedByCustomer(ActiveUser);

        if (customerAccounts is null)
        {
            return 0m;
        }

        List<PersonalAccount> balanceAccounts = customerAccounts.OfType<PersonalAccount>().ToList();

        decimal totalBalance = 0m;

        foreach (PersonalAccount account in balanceAccounts)
        {
            if (!_exchangeRateService.TryGetRate(account.Currency, loanAccount.Currency, out decimal exchangeRate))
            {
                throw new InvalidOperationException("Currency conversion failed.");
            }

            decimal convertedBalance = account.Balance * exchangeRate;

            totalBalance += convertedBalance;
        }

        return totalBalance * 5;
    }
}

