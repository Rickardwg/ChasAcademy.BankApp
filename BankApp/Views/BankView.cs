namespace BankApp.Views;

internal class BankView
{
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
