using BankApp.Enums.Errors;
using Spectre.Console;

namespace BankApp.Views;

internal class AuthenticationView
{
    public string DisplayMenu()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Login",
                "0" => "Exit",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            }));
    }

    public string DisplayBackToMainMenu(bool allowRetry)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .UseConverter(choice => choice switch
            {
                "1" => "Try again",
                "0" => "Main menu",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            });

        if (allowRetry)
        {
            prompt.AddChoice("1");
        }

        prompt.AddChoice("0");
        return AnsiConsole.Prompt(prompt);
    }

    public string ReadUsername()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Username:[/]"));
    }

    public string ReadPassword()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Password:[/]").Secret());
    }
}
