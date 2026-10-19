using BankApp.Models;

namespace BankApp.Views;

internal class AuthenticationView
{

    public string DisplayMenu()
    {
        Console.WriteLine("Welcome! What do you want to do?");
        Console.WriteLine("[1] Login\n" +
                          "[2] Register\n" +
                          "[0] Exit");

        string userChoice = "";
        bool keepRunning = true;
        while (keepRunning)
        {
            userChoice = Console.ReadLine() ?? "";

            switch (userChoice)
            {
                case ("1"):
                    Console.WriteLine("Redirecting to login...");
                    break;
                case ("2"):
                    Console.WriteLine("Redirecting to register...");
                    break;
                case ("0"):
                    Console.WriteLine("Exiting the program...");
                    break;
                default:
                    Console.WriteLine("Please enter one of the valid options.");
                    continue;
            }

            keepRunning = false;

        }

        return userChoice;

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
