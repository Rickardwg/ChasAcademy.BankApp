using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services
{
    internal class TransactionSchedulingService
    {
        private readonly Queue<PendingTransaction> _pendingTransactions = new();
        private readonly object _queueLock = new();

        public void EnqueueTransaction(PendingTransaction transaction)
        {
            lock (_queueLock)
            {
                _pendingTransactions.Enqueue(transaction);
            }
        }
        public PendingTransaction? DequeueTransaction()
        {
            lock (_queueLock)
            {
                if (_pendingTransactions.Count == 0)
                {
                    return null;
                }

                return _pendingTransactions.Dequeue();
            }
        }
    }
}
