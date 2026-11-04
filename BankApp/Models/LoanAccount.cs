namespace BankApp.Models;

public class LoanAccount : BankAccount
{
    public decimal Owed { get; set; }
    public decimal AmountWithdrawn { get; set; } = 0m;
    public decimal InterestRate { get; set; } = 0.08m;
}
