using Spectre.Console;

namespace BankApp.Views;

internal static class ConsoleHeader
{
    public static void DisplayLogo()
    {
        AnsiConsole.Clear();
        var figlet = new FigletText("Bank of Aldor").Color(Color.Gold1).Centered();
        AnsiConsole.Write(figlet);
    }
}
