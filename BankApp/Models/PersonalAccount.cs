namespace BankApp.Models;

public class PersonalAccount : BankAccount
{
    public decimal Balance { get; set; }
    public string AccountNumber { get; set; }
}
