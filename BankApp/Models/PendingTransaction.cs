using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models
{
    internal class PendingTransaction
    {
        public BankAccount SourceAccount { get; }
        public BankAccount DestinationAccount { get; }
        public decimal Amount { get; }
        public DateTimeOffset CreatedAt { get; }

        public PendingTransaction(BankAccount sourceAccount, BankAccount destinationAccount, decimal amount)
        {
            SourceAccount = sourceAccount;
            DestinationAccount = destinationAccount;
            Amount = amount;
            CreatedAt = DateTimeOffset.Now;
        }
    }
}
