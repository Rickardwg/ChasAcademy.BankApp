using BankApp.Enums;
using BankApp.Enums.Errors;
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

    public string DisplayBackToMainMenu(bool allowRetry)
    {
        if (!allowRetry)
        {
            Console.WriteLine("\nOption: \n[0] Main menu");
        }
        else
        {
            Console.WriteLine("\nOptions: \n[1] Try again \n[0] Main menu");
        }
        return Console.ReadLine() ?? "";
    }

    public (string FirstName, string LastName, string EmailAddress, string PhoneNumber, string UserName, string Password) DisplayRegisterMenu()
    {
        Console.WriteLine("\nRegistration form:");
        Console.WriteLine("Please fill in all the following fields.\n");

        Console.Write("First name: ");
        string firstName = Console.ReadLine() ?? "";

        Console.Write("Last name: ");
        string lastName = Console.ReadLine() ?? "";

        Console.Write("Email address: ");
        string emailAddress = Console.ReadLine() ?? "";

        Console.Write("Swedish phone number: ");
        string phoneNumber = Console.ReadLine() ?? "";

        Console.Write("Username: ");
        string userName = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        return (firstName, lastName, emailAddress, phoneNumber, userName, password);
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

    public void DisplayRegistrationError(RegistrationError error)
    {
        var message = error switch
        {
            RegistrationError.UsernameAlreadyExists => "Username is already in use.",
            RegistrationError.InvalidFirstName => "First name is too short.",
            RegistrationError.InvalidLastName => "Last name is too short.",
            RegistrationError.InvalidEmailAddress => "Invalid email address format.",
            RegistrationError.InvalidPhoneNumber => "Only Swedish phone numbers are supported.",
            RegistrationError.InvalidUsername => "Username is too short.",
            RegistrationError.InvalidPassword => "Password is too short.",
            _ => throw new InvalidOperationException("Unexpected display registration error failure.")
        };

        Console.WriteLine(message);
    }
}
