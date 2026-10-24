using BankApp.Models;
using BankApp.Enums;
using BankApp.Repositories;

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
            AccountType.Loan => new LoanAccount { accountType = AccountType.Loan },
            _ => throw new InvalidOperationException()
        };

        account.OwnerId = ActiveUser.Id;
        account.Currency = currency;

        _repository.Add(account);
    }

    public bool TransferMoney(PersonalAccount sourceAccount, PersonalAccount destinationAccount, decimal amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (sourceAccount.Balance < amount)
        {
            return false;
        }

        if (sourceAccount.OwnerId != destinationAccount.OwnerId)
        {
            return false;
        }

        if (ReferenceEquals(sourceAccount, destinationAccount))
        {
            return false;
        }
        if (sourceAccount.Currency != destinationAccount.Currency)
        {
            return false;
        }

        sourceAccount.Balance -= amount;
        sourceAccount.TransactionHistory.Add(new Transaction
        { 
            Type = TransactionType.Transfer,
            Amount = -amount,
            Currency = sourceAccount.Currency,
            RelatedAccountId = destinationAccount.AccountNumber,
            Desciption = $"Transfer to {destinationAccount.AccountNumber}"
        });

        destinationAccount.Balance += amount;
        // amount and currency needs to be handled differently when converting currencies is implemented.
        // use these fields for the received amount and currency.
        // use the "converted" properties for the source values.
        destinationAccount.TransactionHistory.Add(new Transaction
        {
            Type = TransactionType.Transfer,
            Amount = amount,
            Currency = sourceAccount.Currency,
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
}

