using BankApp.Models;

namespace BankApp.Repositories;

internal class AccountRepository
{
    private readonly List<BankAccount> _accounts = [];

    internal void Add(BankAccount account)
    {
        _accounts.Add(account);
    }

    internal int GetNextAccountNumber()
    {
        var number = _accounts.Count + 1;

        while (_accounts.OfType<PersonalAccount>().Any(a => a.AccountNumber == number.ToString()))
        {
            number++;
        }

        return number;
    }

    internal List<BankAccount>? GetBankAccountsOwnedByCustomer(Customer customer)
    {
        if (_accounts.Count <= 0)
        {
            return null;
        }

        List<BankAccount> bankAccounts = _accounts.Where(account => customer.Id == account.OwnerId).ToList();

        return bankAccounts;
    }

    internal List<BankAccount>? GetAllBankAccounts()
    {
        if (_accounts.Count > 0)
        {
            return _accounts;
        }
        else
        {
            return null;
        }
    }
}