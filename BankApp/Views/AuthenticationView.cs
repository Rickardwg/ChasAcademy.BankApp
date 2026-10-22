using BankApp.Enums;
using BankApp.Models;

namespace BankApp.Views;

internal class AuthenticationView
{
    public void DisplayWelcomeMessage()
    {
        Console.WriteLine("Welcome! What do you want to do?");
    }
    public string DisplayMenu()
    {
        Console.WriteLine("\n[1] Login \n[2] Register \n[0] Exit");

        return Console.ReadLine() ?? "";
    }

    public string DisplayBackToMainMenu(User? user)
    {
        if (user is not null && user.IsLocked)
        {
            Console.WriteLine("\nOption: \n[0] Main menu");
        }
        else
        {
            Console.WriteLine("\nOptions: \n[1] Try again \n[0] Main menu");
        }
        return Console.ReadLine() ?? "";
    }

    public Customer DisplayRegisterMenu()
    {
        Customer newCustomer = new Customer();

        Console.WriteLine("\nRegistration form:");
        Console.WriteLine("Please fill in all the following fields.\n");

        Console.Write("First name: ");
        newCustomer.FirstName = Console.ReadLine();

        Console.Write("Last name: ");
        newCustomer.LastName = Console.ReadLine();

        Console.Write("Email: ");
        newCustomer.Email = Console.ReadLine();

        Console.Write("Username: ");
        newCustomer.UserName = Console.ReadLine();

        Console.Write("Password: ");
        newCustomer.Password = Console.ReadLine();

        Console.Write("Phone number: ");
        newCustomer.PhoneNumber = Console.ReadLine();

        return newCustomer;
    }

    public string ReadUsername()
    {
        Console.Write("Username: ");
        string userName = Console.ReadLine() ?? "";

        return userName;
    }

    public string ReadUserPassword()
    {
        Console.Write("Password: ");
        string userPassword = Console.ReadLine() ?? "";

        return userPassword;
    }
}
