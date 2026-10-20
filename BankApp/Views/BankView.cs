using BankApp.Enums;
using BankApp.Models;

namespace BankApp.Views;

internal class BankView
{
    public void DisplayAccounts(List<BankAccount> accounts)
    {
        foreach (var account in accounts)
        {
            Console.WriteLine($"Currency: {account.Currency}");

            if (account is PersonalAccount personalAccount)
            {
                Console.WriteLine($"Balance: {personalAccount.Balance}");
                Console.WriteLine($"Account Number: {personalAccount.AccountNumber}");
            }
            else if (account is LoanAccount loanAccount)
            {
                Console.WriteLine($"Amount Owed: {loanAccount.Owed}");
            }

            // Visual separator between accounts in console window.
            Console.WriteLine("------------------");
        }
    }
	public string DisplayMenu()
	{
		Console.WriteLine("\nMenu: \n[1] Accounts \n[2] Transfer money \n[3] Loans  \n[4] Create account \n[0] Logout");

		string userChoice = "";
		bool isRunning = true;
		while (isRunning)
		{
			userChoice = Console.ReadLine() ?? "";

			switch (userChoice)
			{
				case ("1"):
					Console.WriteLine("Redirecting to accounts...");
					break;
				case ("2"):
					Console.WriteLine("Redirecting to transfer money...");
					break;
				case ("3"):
					Console.WriteLine("Redirecting to Loans...");
					break;
				case ("4"):
					Console.WriteLine("Redirecting to create account...");
					break;
				case ("0"):
					break;
				default:
					Console.WriteLine("\nPlease enter one of the valid options.\n");
					continue;
			}

			isRunning = false;
		}

		return userChoice;
	}
}
