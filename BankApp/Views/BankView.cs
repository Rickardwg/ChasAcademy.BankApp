using BankApp.Models;
using Spectre.Console;

namespace BankApp.Views;

internal class BankView
{
    public string DisplayMenu()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]Main menu:[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "3", "4", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Accounts",
                "2" => "Transfer money",
                "3" => "Loans",
                "4" => "Create account",
                "0" => "Logout",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            })
        );
    }

    public void DisplayAccounts(List<BankAccount> accounts)
    {

        var table = new Table();

        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Gold1);
        table.Title("[bold gold1]Bank Accounts[/]");

        table.AddColumn("[bold]Account Type[/]");
        table.AddColumn("[bold]Currency[/]");
        table.AddColumn("[bold]Balance / Amount Owed[/]");
        table.AddColumn("[bold]Account Number[/]");

        foreach (var account in accounts)
        {
            switch (account)
            {
                case SavingsAccount savingAccount:
                    table.AddRow(
                        "[blue]Savings[/]",
                        savingAccount.Currency.ToString(),
                        $"[blue]{savingAccount.Balance:N2}[/]",
                        savingAccount.AccountNumber.ToString()
                    );
                    break;
                case PersonalAccount personalAccount:
                    table.AddRow(
                        "[green]Personal[/]",
                        personalAccount.Currency.ToString(),
                        $"[green]{personalAccount.Balance:N2}[/]",
                        personalAccount.AccountNumber.ToString()
                    );
                    break;
                case LoanAccount loanAccount:
                    table.AddRow(
                        "[red]Loan[/]",
                        loanAccount.Currency.ToString(),
                        $"[red]{loanAccount.Owed:N2}[/]",
                        "[dim]N/A[/]"
                    );
                    break;
            }
        }

        AnsiConsole.Write(table);
    }

    public T? ReadEnumChoice<T>(string title) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();

        var choices = values
            .Select(value => value.ToString())
            .ToList();

        choices.Add("Go back");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold gold1]{title}:[/]")
                .HighlightStyle("bold black on gold1")
                .AddChoices(choices)
        );

        if (selected == "Go back")
        {
            return null;
        }

        return Enum.Parse<T>(selected);
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



