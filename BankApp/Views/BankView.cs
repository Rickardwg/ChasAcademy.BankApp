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
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]Select transfer type:[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Transfer between your own accounts",
                "2" => "Transfer to another customer's account",
                "0" => "Go back",
                _ => throw new InvalidOperationException("Unexpected transfer choice.")
            })
        );
    }

    public int SelectOwnAccount(List<BankAccount> accounts, string prompt)
    {
        var choices = accounts.Select((account, index) => new
        {
            Index = index,
            Display = account switch
            {
                SavingsAccount s =>
                    $"[[Savings]] Account: {s.AccountNumber} | Balance: {s.Balance:N2} {s.Currency}",

                PersonalAccount p =>
                    $"[[Personal]] Account: {p.AccountNumber} | Balance: {p.Balance:N2} {p.Currency}",

                LoanAccount l =>
                    $"[[Loan]] Account: N/A | Owed: {l.Owed:N2} {l.Currency}",

                _ => $"Unknown account: {account.AccountNumber}"
            }
        }).ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold gold1]{prompt}[/]")
                .HighlightStyle("bold black on gold1")
                .PageSize(10)
                .AddChoices(choices.Select(x => x.Display))
        );

        return choices.First(x => x.Display == selected).Index;
    }

    public int SelectOtherCustomerAccount(List<PersonalAccount> accounts, string prompt)
    {
        Console.WriteLine($"\n{prompt}");

        for (int i = 0; i < accounts.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] Account number: {accounts[i].AccountNumber}");
        }

        while (true)
        {
            Console.Write("\nSelect account number: ");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection >= 1 && selection <= accounts.Count)
            {
                return selection - 1;
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\nPlease enter a number between 1 and {accounts.Count}.");
            Console.ResetColor();
        }
    }

    public decimal GetTransferAmount()
    {
        return AnsiConsole.Ask<decimal>(
            "[bold gold1]Enter amount to transfer:[/]"
        );
    }
    public void DisplayTransferResult(bool success)
    {
        if (success)
        {
            var panel = new Panel(
                new Markup("[green] The transfer has been completed successfully.[/]")
            )
            .Header("[green bold]Transfer Successful[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.DarkGreen)
            .Padding(2, 1);

            AnsiConsole.Write(panel);
        }
        else
        {
            var panel = new Panel(
                new Rows(
                    new Markup("[red]The transfer could not be completed.[/]"),
                    new Markup("[grey]Please check the account balance and transfer amount.[/]")
                )
            )
            .Header("[red bold]X Transfer failed[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.DarkRed)
            .Padding(2, 1);

            AnsiConsole.Write(panel);
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
    public void DisplayInterestPreview(decimal depositAmount, decimal interestRate, int years, decimal expectedInterest)
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
        Console.Write("Enter Years: ");
        int.TryParse(Console.ReadLine(), out int years);

        return years;
    }
    public void DisplayInvalidInterestInput()
    {
        Console.WriteLine("Invalid amount or number of years.");
    }

    public void DisplayLoanPreview
        (decimal loanAmount, decimal interestRate, int years, decimal totalInterest)
    {
        Console.WriteLine("\n\tLoan Preview");
        Console.WriteLine("");

        Console.WriteLine($"Loan Amount:{loanAmount:F2}");
        Console.WriteLine($"Annual interest: {interestRate:P2}");
        Console.WriteLine($"Loan Duration: {years} years");

        Console.WriteLine($"Total Interest:{totalInterest:F2}");
        Console.WriteLine($"Total Amount Owed: {loanAmount + totalInterest:F2}");
    }

    public decimal GetLoanAmount()
    {
        Console.Write("Enter Loan Amount:");
        decimal.TryParse(Console.ReadLine(), out decimal amount);

        return amount;
    }

    public bool ConfirmLoan()
    {
        while (true)
        {
            Console.Write("Would you like to confirm this loan? (Y/N): ");

            string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (input == "Y")
            {
                return true;
            }
            else if (input == "N")
            {
                return false;
            }

            Console.WriteLine("Invalid input! Please answere with Y or N.");
        }
    }
    public void DisplayLoanResult(bool success)
    {
        if (success)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nYour loan has been registered successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nThe loan could not be completed.");
        }

        Console.ResetColor();
    }

    public void DisplayInvalidLoanInput()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\nInvalid loan amount or number of years. ");
        Console.ResetColor();
    }
}



