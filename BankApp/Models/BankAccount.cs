using BankApp.Enums;

namespace BankApp.Models;

public class BankAccount
{
    public Guid OwnerId { get; set; }

    public List<Transaction> TransactionHistory { get; set; } = [];
    public Currency Currency { get; set; }
}
