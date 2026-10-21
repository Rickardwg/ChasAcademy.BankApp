using BankApp.Models;
﻿using BankApp.Enums;
using BankApp.Repositories;

namespace BankApp.Services;

internal class BankService(AccountRepository repository)
{
    private readonly AccountRepository _repository = repository;

    internal Customer ActiveUser {  get; private set; }

    internal void SetActiveUser(Customer user)
    {
        ActiveUser = user;
    }

    internal void CreateNewAccount(AccountType type, Currency currency)
    {
        BankAccount account = type switch
        {
            AccountType.Personal => new PersonalAccount { AccountNumber = _repository.GetNextAccountNumber().ToString() },
            AccountType.Savings => new SavingsAccount { AccountNumber = _repository.GetNextAccountNumber().ToString() },
            AccountType.Loan => new LoanAccount(),
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
        destinationAccount.Balance += amount;

        return true;
    }
}

