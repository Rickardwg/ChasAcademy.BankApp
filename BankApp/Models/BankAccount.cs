using BankApp.Enums;

namespace BankApp.Models;

public class BankAccount
{
    public Guid OwnerId { get; set; }

    // TODO Add stack type
    // public Stack<> TransactionHistory { get; set; }
    public Currency Currency { get; set; }
    public AccountType accountType { get; set; }
}
