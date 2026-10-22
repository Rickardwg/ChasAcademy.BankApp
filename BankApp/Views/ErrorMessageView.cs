using BankApp.Enums;

namespace BankApp.Views;

public class ErrorMessageView
{
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
                Console.WriteLine("\nPlease enter one of the valid options.");
                Console.ResetColor();

                break;
            case ErrorMessages.NoBankAccountFound:

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nYou don't have any registered bank accounts yet.");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Please create an account to get started.");
                Console.ResetColor();

                break;
        }
    }
}

