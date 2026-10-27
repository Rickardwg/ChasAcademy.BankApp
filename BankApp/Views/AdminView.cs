using BankApp.Models;

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
        Console.WriteLine("\nMain menu:");
        Console.WriteLine("[1] Manage users \n[2] Manage interest \n[3] Manage currency \n[0] Logout");
        return Console.ReadLine() ?? "";
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
