using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services
{
    internal class TransactionSchedulingService(BankService bankService)
    {
        private readonly BankService _bankService = bankService;
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
        private TimeSpan CalculateDelayUntilNextQuarter()
        {
            var now = DateTimeOffset.Now;

            int remainder = now.Minute % 15;

            int minutesUntilNextQuarter = 15 - remainder;

            int secondsUntilNextQuarter = minutesUntilNextQuarter * 60 - now.Second;

            TimeSpan delay = TimeSpan.FromSeconds(secondsUntilNextQuarter)
                           - TimeSpan.FromMilliseconds(now.Millisecond);

            return delay;
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TimeSpan delay = CalculateDelayUntilNextQuarter();

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                PendingTransaction? transaction = DequeueTransaction();

                while (transaction != null)
                {
                    _bankService.TransferMoney(
                         transaction.SourceAccount, 
                         transaction.DestinationAccount, 
                         transaction.Amount);

                    transaction = DequeueTransaction();
                }
            }
        }
    }
}
