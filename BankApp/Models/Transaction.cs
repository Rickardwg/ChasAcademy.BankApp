using BankApp.Enums;

namespace BankApp.Models;

public class Transaction
{
    //public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }

    public decimal? ConvertedAmount { get; set; }
    public Currency? ConvertedCurrency { get; set; }
    public decimal? ExchangeRate { get; set; }

    public string? RelatedAccountId { get; set; }
    public string? Desciption { get; set; }
}
