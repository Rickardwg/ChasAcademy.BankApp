using BankApp.Enums;
using BankApp.Models;

namespace BankApp.Views;

internal class BankView
{
    public void DisplayAccounts(List<BankAccount> accounts)
    {
        foreach (var account in accounts)
        {
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
    public string DisplayMenu()
    {
        Console.WriteLine("\nMenu: \n[1] Accounts \n[2] Transfer money \n[3] Loans  \n[4] Create account \n[0] Logout");

        string userChoice = "";
        bool isRunning = true;
        while (isRunning)
        {
            userChoice = Console.ReadLine() ?? "";

            switch (userChoice)
            {
                case ("1"):
                    Console.WriteLine("Redirecting to accounts...");
                    break;
                case ("2"):
                    Console.WriteLine("Redirecting to transfer money...");
                    break;
                case ("3"):
                    Console.WriteLine("Redirecting to Loans...");
                    break;
                case ("4"):
                    Console.WriteLine("Redirecting to create account...");
                    break;
                case ("0"):
                    break;
                default:
                    Console.WriteLine("\nPlease enter one of the valid options.\n");
                    continue;
            }

            isRunning = false;
        }

        return userChoice;
    }

    internal AccountType? ReadAccountTypeChoice()
    {
        var types = Enum.GetValues<AccountType>();

        Console.WriteLine("Account Types");

        for (int i = 0; i < types.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {types[i]}");
        }

        while (true)
        {
            Console.WriteLine("Select account type or enter '0' to go back:");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    return null; // EXIT
                }

                if (choice >= 1 && choice <= types.Length)
                {
                    return types[choice - 1];
                }
            }

            Console.WriteLine("Invalid choice, try again.");
        }
    }

    internal Currency? ReadCurrencyChoice()
    {
        var currencies = Enum.GetValues<Currency>();

        Console.WriteLine("Available Currencies");

        for (int i = 0; i < currencies.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {currencies[i]}");
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

                if (choice >= 1 && choice <= currencies.Length)
                {
                    return currencies[choice - 1];
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
            Console.WriteLine("Transfer successful!");
        }
        else
        {
            Console.WriteLine("Transfer failed!");
        }
    }

    public void DisplayTransactionHistory(List<Transaction> transactions)
    {
        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions registered.");
            return;
        }

        Console.WriteLine("Transactions:");

        foreach (var transaction in transactions.Reverse<Transaction>())
        {
            Console.WriteLine($"{transaction.Date:d}");

            if (transaction.Desciption is not null)
            {
                Console.WriteLine(transaction.Desciption);
            }

            Console.WriteLine(transaction.Amount);

            Console.WriteLine(transaction.Type);

            Console.WriteLine();
        }
    }
    public void DisplayInterestPreview
       (decimal depositAmount, decimal interestRate, int years, decimal expectedInterest)
    {
        Console.WriteLine("\n\tSavings Account Interest Preview");

        Console.WriteLine($"Deposit Amount: {depositAmount:F2}");
        Console.WriteLine($"Yearly Interest Rate: {interestRate:P2}");
        Console.WriteLine($"Number of Years: {years}");
        Console.WriteLine($"Expected Interest: {expectedInterest:F2}");
        Console.WriteLine($"Estimated Total: {depositAmount + expectedInterest:F2}");
    }

    public decimal GetDepositAmount()
    {
        Console.Write("Enter amount to deposit: ");
        decimal.TryParse(Console.ReadLine(), out decimal amount);

        return amount;
    }
    public int GetInterestYears()
    {
        Console.Write("Enter number of years: ");
        int.TryParse(Console.ReadLine(), out int years);

        return years;
    }
    public void DisplayInvalidInterestInput()
    {
        Console.WriteLine("Invalid amount or number of years.");
    }
}
 


