using BankApp.Models;
using BankApp.Repositories;

namespace BankApp.Services;

internal class BankService(AccountRepository repository)
{
    private readonly AccountRepository _repository = repository;

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

        return true ;
    }
}
