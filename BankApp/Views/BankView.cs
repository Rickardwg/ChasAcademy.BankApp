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
        return AnsiConsole.Prompt(new SelectionPrompt<int>()
            .Title($"[bold gold1]{prompt}[/]")
            .HighlightStyle("bold black on gold1")
            .PageSize(10)
            .AddChoices(Enumerable.Range(1, accounts.Count))
            .UseConverter(selection => $"Account number: {accounts[selection - 1].AccountNumber}")
        ) - 1;
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
            var emptyPanel = new Panel(
                new Markup("[yellow]No transactions registered.[/]")
            )
            .Header("[yellow bold]TRANSACTION HISTORY[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Gold1)
            .Padding(2, 1);

            AnsiConsole.Write(emptyPanel);
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Gold1)
            .Title("[bold gold1]TRANSACTION HISTORY[/]")
            .AddColumn("[bold]Date[/]")
            .AddColumn("[bold]Description[/]")
            .AddColumn("[bold]Amount[/]")
            .AddColumn("[bold]Type[/]");

        foreach (var transaction in transactions.AsEnumerable().Reverse())
        {
            table.AddRow(
                transaction.Date.ToString("d"),
                transaction.Desciption ?? "-",
                transaction.Amount.ToString("F2"),
                transaction.Type.ToString()
            );
        }

        AnsiConsole.Write(table);
    }
    public void DisplayInterestPreview(decimal depositAmount, decimal interestRate, int years, decimal expectedInterest)
    {
        var panel = new Panel(
            new Markup(
                $"[bold]Deposit Amount:[/] {depositAmount:F2}\n" +
                $"[bold]Yearly Interest Rate:[/] {interestRate:P2}\n" +
                $"[bold]Number of Years:[/] {years}\n\n" +
                $"[bold]Expected Interest:[/] {expectedInterest:F2}\n" +
                $"[bold gold1]Estimated Total:[/] [gold1]{depositAmount + expectedInterest:F2}[/]"
            )
        )
        .Header("[bold gold1]SAVINGS ACCOUNT INTEREST PREVIEW[/]", Justify.Center)
        .RoundedBorder()
        .BorderColor(Color.Gold1)
        .Padding(2, 1);

        AnsiConsole.Write(panel);
    }

    public decimal GetDepositAmount()
    {
        return AnsiConsole.Ask<decimal>(
            "[bold gold1]Enter deposit amount:[/]"
        );
    }
    public int GetInterestYears()
    {
        return AnsiConsole.Ask<int>(
            "[bold gold1]Enter loan duration (years):[/]"
        );
    }
    public void DisplayInvalidInterestInput()
    {
        var panel = new Panel(
            new Markup("[red]Invalid loan amount or duration.[/]")
        )
        .Header("[red bold]X[/] [red]INVALID LOAN INPUT[/]", Justify.Center)
        .RoundedBorder()
        .BorderColor(Color.DarkRed)
        .Padding(2, 1);

        AnsiConsole.Write(panel);
    }

    public void DisplayLoanPreview(decimal loanAmount, decimal interestRate, int years, decimal totalInterest)
    {
        var panel = new Panel(
            new Markup(
                $"[bold]Loan Amount:[/] {loanAmount:F2}\n" +
                $"[bold]Annual Interest:[/] {interestRate:P2}\n" +
                $"[bold]Loan Duration:[/] {years} years\n\n" +
                $"[bold]Total Interest:[/] {totalInterest:F2}\n" +
                $"[bold gold1]Total Amount Owed:[/] [gold1]{loanAmount + totalInterest:F2}[/]"
            )
        )
        .Header("[bold gold1]Loan Preview[/]", Justify.Center)
        .RoundedBorder()
        .BorderColor(Color.Gold1)
        .Padding(2, 1);

        AnsiConsole.Write(new Padder(panel, new Padding(0, 1)));
    }

    public decimal GetLoanAmount()
    {
        return AnsiConsole.Ask<decimal>(
            "[bold gold1]Enter loan amount:[/]"
        );
    }

    public bool ConfirmLoan()
    {
        return AnsiConsole.Confirm(
            "[bold gold1]Would you like to confirm this loan?[/]"
        );
    }

    public void DisplayLoanResult(bool success)
    {
        var panel = new Panel(
            new Markup(success
                ? "[green]Your loan has been registered successfully.[/]"
                : "[red]The loan could not be completed.[/]"
            )
        )
        .Header(success ? "[green bold]Loan Successful[/]" : "[red bold]Loan Failed[/]", Justify.Center)
        .RoundedBorder()
        .BorderColor(success ? Color.DarkGreen : Color.DarkRed)
        .Padding(2, 1);

        AnsiConsole.Write(new Padder(panel, new Padding(0, 1)));
    }

    public void DisplayInvalidLoanInput()
    {
        var panel = new Panel(
            new Markup("[red]Invalid loan amount or number of years.[/]")
        )
        .Header("[red bold] X [/][red]INVALID LOAN DETAILS[/]", Justify.Center)
        .RoundedBorder()
        .BorderColor(Color.DarkRed)
        .Padding(2, 1);

        AnsiConsole.Write(panel);
    }
}



