using BankApp.Enums.Errors;
using Spectre.Console;

namespace BankApp.Views;

internal class AuthenticationView
{
    public void DisplayWelcomeMessage()
    {
        var figlet = new FigletText("Bank of Aldor") { Justification = Justify.Center }.Color(Color.Gold1);
        AnsiConsole.Write(figlet);
    }

    public string DisplayMenu()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]What would you like to do?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("1", "2", "0")
            .UseConverter(choice => choice switch
            {
                "1" => "Login",
                "2" => "Register",
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

    public (string FirstName, string LastName, string EmailAddress, string PhoneNumber, string UserName, string Password) DisplayRegisterMenu()
    {
        AnsiConsole.MarkupLine("[bold gold1]Registration form:[/]");
        AnsiConsole.MarkupLine("[italic grey]Please fill in all the following fields.[/]");
        AnsiConsole.WriteLine();

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
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Username:[/]"));
    }

    public string ReadPassword()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Password:[/]").Secret());
    }

    public string ReadFirstName()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]First name:[/]"));
    }

    public string ReadLastName()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Last name:[/]"));
    }

    public string ReadEmailAddress()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Email address:[/]"));
    }

    public string ReadPhoneNumber()
    {
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Swedish phone number:[/]"));
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
