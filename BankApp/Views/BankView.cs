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

    internal string DisplayTransferMoneyMenu()
    {
        Console.WriteLine("Select transfer type:\n[1] Transfer between your own accounts\n[2] Transfer to another customer's account\n[0] Go back");

        string userChoice;
        while (true)
        {
            userChoice = Console.ReadLine() ?? "";

            if (userChoice == "1" || userChoice == "2" || userChoice == "0")
            {
                break;
            }

            Console.WriteLine("Please enter one of the valid options.");
        }

        return userChoice;
    }

    public int SelectAccount(List<PersonalAccount> accounts, string prompt)
    {
        Console.WriteLine($"\n{prompt}");

        for (int i = 0; i < accounts.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {accounts[i].accountType} account");
            Console.WriteLine($"Account number: {accounts[i].AccountNumber} | Balance: {accounts[i].Balance} {accounts[i].Currency}");
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



