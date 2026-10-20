using BankApp.Models;

namespace BankApp.Repositories;

internal class AccountRepository
{
    private readonly List<BankAccount> _accounts = [];

    internal int GetNextAccountNumber()
    {
        var number = _accounts.Count + 1;

        while (_accounts.OfType<PersonalAccount>().Any(a => a.AccountNumber == number.ToString()))
        {
            number++;
        }

        return number;
    }
}