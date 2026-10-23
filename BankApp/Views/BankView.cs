using BankApp.Enums;
using BankApp.Models;

namespace BankApp.Views;

internal class BankView
{
    public string DisplayMenu()
    {
        Console.WriteLine("\nMenu: \n[1] Accounts \n[2] Transfer money \n[3] Loans  \n[4] Create account \n[0] Logout");

        return Console.ReadLine() ?? "";
    }

    public void DisplayAccounts(List<BankAccount> accounts)
    {
        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.accountType} account");
            Console.WriteLine($"Currency: {account.Currency}");

            if (account is PersonalAccount personalAccount)
            {
                Console.WriteLine($"Balance: {personalAccount.Balance}");
                Console.WriteLine($"Account Number: {personalAccount.AccountNumber}");
            }
            else if (account is LoanAccount loanAccount)
            {
                Console.WriteLine($"Amount Owed: {loanAccount.Owed}");
            }

            // Visual separator between accounts in console window.
            Console.WriteLine("------------------");
        }
    }

    public T? ReadEnumChoice<T>(string title) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        Console.WriteLine($"\n{title}:");

        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {values[i]}");
        }

        while (true)
        {
            Console.WriteLine("Select currency or enter '0' to go back:");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    return null; // EXIT
                }

                if (choice >= 1 && choice <= values.Length)
                {
                    return values[choice - 1];
                }
            }

            Console.WriteLine("Invalid choice, try again.");
        }
    }

    public int SelectAccount(List<PersonalAccount> accounts, string prompt)
    {
        Console.WriteLine($"\n\t{prompt}");

        for (int i = 0; i < accounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {accounts[i].AccountNumber} - " +
                              $"{accounts[i].Balance} {accounts[i].Currency}");
        }

        Console.Write("\nSelect account number: ");

        int.TryParse(Console.ReadLine(), out int selection);

        return selection - 1;
    }
    public decimal GetTransferAmount()
    {
        Console.Write("Enter amount to transfer: ");

        decimal.TryParse(Console.ReadLine(), out decimal amount);

        return amount;
    }
    public void DisplayTransferResult(bool success)
    {
        if (success)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nThe transfer has been completed successfully.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nThe transfer could not be completed.");
            Console.ResetColor();

            Console.WriteLine("Please check the account balance and transfer amount.");
        }
    }
}




