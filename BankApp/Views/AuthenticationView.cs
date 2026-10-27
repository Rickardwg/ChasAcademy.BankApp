using BankApp.Enums;
using BankApp.Enums.Errors;
using BankApp.Models;

namespace BankApp.Views;

internal class AuthenticationView
{
    public void DisplayWelcomeMessage()
    {
        var figlet = new FigletText("Bank of Aldor") { Justification = Justify.Center }.Color(Color.Yellow);
        AnsiConsole.Write(figlet);
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

        string firstName = ReadFirstName();
        string lastName = ReadLastName();
        string emailAddress = ReadEmailAddress();
        string phoneNumber = ReadPhoneNumber();
        string userName = ReadUsername();
        string password = ReadPassword();

        return (firstName, lastName, emailAddress, phoneNumber, userName, password);
    }

    public string ReadUsername()
    {
        Console.Write("Username: ");
        string userName = Console.ReadLine() ?? "";

        return userName;
    }

    public string ReadPassword()
    {
        Console.Write("Password: ");
        string userPassword = Console.ReadLine() ?? "";

        return userPassword;
    }

    public string ReadFirstName()
    {
        Console.Write("First Name: ");
        string firstName = Console.ReadLine() ?? "";

        return firstName;
    }

    public string ReadLastName()
    {
        Console.Write("Last Name: ");
        string lastName = Console.ReadLine() ?? "";

        return lastName;
    }

    public string ReadEmailAddress()
    {
        Console.Write("Email Address: ");
        string emailAddress = Console.ReadLine() ?? "";

        return emailAddress;
    }

    public string ReadPhoneNumber()
    {
        Console.Write("Swedish Phone Number: ");
        string phoneNumber = Console.ReadLine() ?? "";

        return phoneNumber;
    }

    public string RetryRegistrationPrompt()
    {
        while (true)
        {
            Console.Write("Correct this field? [y] Yes / [n] Cancel registration and return to menu: ");
            var input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (input == "y" || input == "n")
            {
                return input;
            }

            Console.WriteLine("Invalid choice, try again.");
        }
    }

    public void DisplayRegistrationError(RegistrationError error)
    {
        var message = error switch
        {
            RegistrationError.UsernameAlreadyExists => "Username is already in use.",
            RegistrationError.InvalidFirstName => "First name must contain at least 2 characters.",
            RegistrationError.InvalidLastName => "Last name must contain at least 4 characters.",
            RegistrationError.InvalidEmailAddress => "Invalid email address format.",
            RegistrationError.InvalidPhoneNumber => "Only Swedish phone numbers are supported.",
            RegistrationError.InvalidUsername => "Username must contain at least 5 characters.",
            RegistrationError.InvalidPassword => "Password must contain at least 7 characters.",
            _ => throw new InvalidOperationException("Unexpected display registration error failure.")
        };

        Console.WriteLine(message);
    }
}
