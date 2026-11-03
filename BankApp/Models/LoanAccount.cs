namespace BankApp.Models;

public class LoanAccount : BankAccount
{
    public decimal Owed { get; set; }
    public decimal InterestRate { get; set; } = 0.08m;
}
