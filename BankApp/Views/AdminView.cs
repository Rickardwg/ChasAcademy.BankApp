using BankApp.Enums.Errors;
using BankApp.Models;
using Spectre.Console;

namespace BankApp.Views;

internal class AdminView
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

    public void DisplayRegisterHeader()
    {
        AnsiConsole.MarkupLine("[bold gold1]Registration form:[/]");
        AnsiConsole.MarkupLine("[italic grey]Please fill in all the following fields.[/]");
        AnsiConsole.WriteLine();
    }

    public (string FirstName, string LastName, string EmailAddress, string PhoneNumber, string UserName, string Password) DisplayRegisterCustomerMenu()
    {
        DisplayRegisterHeader();

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
        return AnsiConsole.Prompt(new TextPrompt<string>("[bold gold1]Password:[/]"));
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
        return AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[bold gold1]Correct this field?[/]")
            .HighlightStyle("bold black on gold1")
            .AddChoices("y", "n")
            .UseConverter(choice => choice switch
            {
                "y" => "Yes",
                "n" => "Cancel registration and return to menu",
                _ => throw new InvalidOperationException("Unexpected menu choice.")
            }));
    }

    public void DisplayRegistrationSuccess(string username)
    {
        AnsiConsole.MarkupLine($"[green]User[/] '{Markup.Escape(username)}' [green]created successfully.[/]");
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

        AnsiConsole.MarkupLine($"[red1]{message}[/]");
    }
}
