using BankApp.Enums;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace BankApp.Views;

public class ErrorMessageView
{
    private void DisplayErrorPanel(string? icon, string header, params IRenderable[] content)
    {
        var panel = new Panel(new Rows(content))
       .Header($"{(icon != null ? $"[red bold]{icon}[/]" : "")} [red]{Markup.Escape(header)} [/]", Justify.Center)
       .RoundedBorder()
       .BorderColor(Color.DarkRed)
       .Padding(2, 1);

        AnsiConsole.Write(panel);
    }

    public void DisplayErrorMessage(ErrorMessages errorMessages)
    {
        switch (errorMessages)
        {
            case ErrorMessages.LockedAccount:
                {
                    DisplayErrorPanel(
                        null,
                        "SECURITY ALERT",
                        new Markup("[red]Login blocked after three consecutive failed attempts.[/]"),
                        new Markup(
                            "[grey]Access to banking services has been restricted.[/]\n" +
                            "[grey]Please contact your bank for further information and assistance.[/]"
                        )
                    );
                    break;
                }
            case ErrorMessages.UserNotFound:
                {
                    DisplayErrorPanel(
                        "X",
                        "LOGIN FAILED",
                        new Markup("[red]The username or password you entered is incorrect.[/]"),
                        new Markup("[grey]Please check your details and try again.[/]")
                    );
                    break;
                }
            case ErrorMessages.InvalidOption:
                {
                    DisplayErrorPanel(
                        "X",
                        "INVALID OPTION",
                        new Markup("[red]Please enter one of the valid options.[/]")
                    );
                    break;
                }
            case ErrorMessages.NoBankAccountFound:
                {
                    DisplayErrorPanel(
                        "X",
                        "NO BANK ACCOUNTS",
                        new Markup("[red]You don't have any registered bank accounts yet.[/]"),
                        new Markup("[grey]Please create an account to get started.[/]")
                    );
                }
                break;
            case ErrorMessages.NotEnoughAccounts:
                {
                    DisplayErrorPanel(
                        "X",
                        "TRANSFER NOT POSSIBLE",
                        new Markup("[red]No transfer possible.[/]"),
                        new Markup("[red]You need at least two accounts to make a transfer.[/]")
                    );
                }
                break;
            case ErrorMessages.SameSourceAndDestinationAccount:
                {
                    DisplayErrorPanel(
                        "X",
                        "INVALID TRANSFER",
                        new Markup("[red]Source and destination accounts must be different.[/]")
                    );
                    break;
                }
        }
    }
}

