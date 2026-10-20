using BankApp.Enums;

namespace BankApp.Views;

internal class BankView
{
	public string DisplayMenu()
	{
		// TODO Add real username
		Console.WriteLine("Hello, [username]!");
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

	internal AccountType? ReadAccountTypeChoice()
	{
		var types = Enum.GetValues<AccountType>();

        Console.WriteLine("Account Types");
		
		for (int i = 0; i < types.Length; i++)
		{
            Console.WriteLine($"[{i+1}] {types[i]}");
		}

		while (true)
		{
            Console.WriteLine("Select account type or enter '0' to go back:");
            var input = Console.ReadLine();

			if (int.TryParse(input, out int choice))
			{
				if (choice == 0)
				{
					return null; // EXIT
				}

				if (choice >= 1 && choice <= types.Length)
				{
                    return types[choice - 1];
                }
			}

            Console.WriteLine("Invalid choice, try again.");
		}
	}

    internal Currency? ReadCurrencyChoice()
    {
        var currencies = Enum.GetValues<Currency>();

        Console.WriteLine("Available Currencies");

        for (int i = 0; i < currencies.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {currencies[i]}");
        }

        while (true)
        {
            Console.WriteLine("Select currency or enter '0' to go back:");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    return null; // EXIT
                }

                if (choice >= 1 && choice <= currencies.Length)
                {
                    return currencies[choice - 1];
                }
            }

            Console.WriteLine("Invalid choice, try again.");
        }
    }
}