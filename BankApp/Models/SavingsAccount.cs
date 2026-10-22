namespace BankApp.Models;

public class SavingsAccount : PersonalAccount
{
    public decimal InterestRate { get; set; } = 0.05m;
}
