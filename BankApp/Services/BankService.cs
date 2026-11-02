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

        if (sourceAccount is not PersonalAccount source) // also covers SavingsAccount
        {
            // TODO: display error:
            Console.WriteLine("You can't transfer money from a loan account"); // TEMP
            return false;
        }

        if (source.Balance < amount)
        {
            // TODO: display error:
            Console.WriteLine("Insufficient funds"); // TEMP
            return false;
        }

        if (ReferenceEquals(sourceAccount, destinationAccount))
        {
            // TODO: specific error: same source and destination account
            return false;
        }

        if (!_exchangeRateService.TryGetRate(sourceAccount.Currency, destinationAccount.Currency, out decimal exchangeRate))
        {
            return false;
        }

        if (destinationAccount is not PersonalAccount destination) // only personal and savings accounts can receive money for now
        {
            // TODO: once the rules for Owed are decided,
            // accept destinationAccount being a loan account
            Console.WriteLine("You can't transfer money to a loan account"); // TEMP
            return false;
        }

        decimal destinationAmount = amount * exchangeRate;
        bool hasConverted = sourceAccount.Currency != destinationAccount.Currency;

        source.Balance -= amount;
        destination.Balance += destinationAmount;

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
}

