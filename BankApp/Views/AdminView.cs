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
        Console.WriteLine("Manage users:");
        Console.WriteLine("[1] Add \n[2] Change \n[3] Remove \n[0] Main menu");
        return Console.ReadLine() ?? "";
    }

    public string DisplayUserTypeMenu()
    {
        Console.WriteLine("\nChoose user:");
        Console.WriteLine("[1] Admin \n[2] Customer \n[9] Go back\n[0] Main menu");
        return Console.ReadLine() ?? "";
    }
}
