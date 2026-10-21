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
        Console.WriteLine("[1] Login \n[2] Register \n[0] Exit");

        return Console.ReadLine() ?? "";
    }


    public string DisplayBackToMainMenu(User? user)
    {
        if (user is not null && user.IsLocked)
        {
            Console.WriteLine("Option: \n[0] Main menu");
        }
        else
        {
            Console.WriteLine("Options: \n[1] Try again \n[0] Main menu");
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

    public void DisplayErrorMessage(ErrorMessages errorMessages)
    {
        switch (errorMessages)
        {
            case ErrorMessages.LockedAccount:

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYour account has been locked after unsuccessful attempts.");
                Console.ResetColor();

                Console.WriteLine("\nFor your security, access to your account has been temporarily restricted.");

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Please contact your bank for more information and assistance with unlocking your account.");
                Console.ResetColor();

                break;

            case ErrorMessages.UserNotFound:

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nThe username or password you entered is incorrect.");
                Console.ResetColor();

                Console.WriteLine("Please check your details and try again.");
                break;

            case ErrorMessages.InvalidOption:

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nPlease enter one of the valid options.\n");
                Console.ResetColor();

                break;
        }
    }
}
