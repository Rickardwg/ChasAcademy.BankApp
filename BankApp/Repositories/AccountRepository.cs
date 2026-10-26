using BankApp.Models;
using System.Runtime.CompilerServices;

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
            return null; // TODO: error message about no accounts in the bank
        }

        List<BankAccount> customerOwnedAccounts = [];

        foreach (var account in _accounts)
        {
            if (account.OwnerId == customer.Id)
            {
                customerOwnedAccounts.Add(account);
            }
        }

        if (customerOwnedAccounts.Count <= 0)
        {
            return null; // TODO: error message about that the customer has no accounts
        }

        return customerOwnedAccounts;
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