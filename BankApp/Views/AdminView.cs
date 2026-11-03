using BankApp.Models;
using Spectre.Console;

namespace BankApp.Views;

public class AdminView
{
    public void DisplayWelcomeMessage(User user)
    {
        Console.WriteLine("\nAdmin portal.");
        Console.WriteLine($"Welcome {user.UserName}!");
    }

    public string DisplayMenu()
    {
        // EXTRA: Manage Interest
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Manage Users",
                "2" => "Manage Currencies",
                "0" => "Logout",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            }));
    }

    public string DisplayUserManagementMenu()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "3", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Add User",
                "2" => "Edit User",
                "3" => "Remove User",
                "0" => "Go Back",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            }));
    }

    public string DisplayUserTypeMenu()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Add Admin",
                "2" => "Add Customer",
                "0" => "Go Back",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            }));
    }
}
