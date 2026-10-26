using BankApp.Models;

namespace BankApp.Views;

public class AdminView
{
    public void DisplayWelcomeMessage(User user)
    {
        Console.WriteLine($"\nWelcome {user.UserName}!");
    }

    public string DisplayMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("[1] Employees \n[2] Customers \n[0] Logout");
        return Console.ReadLine() ?? "";
    }

    public string DisplayManagementMenu(string entityName)
    {
        Console.WriteLine("\nMenu");
        Console.WriteLine($"[1] Add new {entityName} \n[2] Remove {entityName} \n[3] Show {entityName}s \n[0] Main menu");
        return Console.ReadLine() ?? "";
    }
}
