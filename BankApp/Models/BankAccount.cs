using BankApp.Enums;

namespace BankApp.Models;

public class BankAccount
{
    public Guid OwnerId { get; set; }
    public string AccountNumber { get; set; }

    public List<Transaction> TransactionHistory { get; set; } = [];
    public Currency Currency { get; set; }
    public AccountType accountType { get; set; }
}
