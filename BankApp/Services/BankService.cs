using BankApp.Enums;
using BankApp.Models;
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
}