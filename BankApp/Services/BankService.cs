using BankApp.Enums;
using BankApp.Models;
using BankApp.Repositories;
using System.Security.Principal;

namespace BankApp.Services;

internal class BankService(AccountRepository repository, ExchangeRateService exchangeRateService)
{
    private readonly AccountRepository _repository = repository;
    private readonly ExchangeRateService _exchangeRateService = exchangeRateService;

    internal Customer ActiveUser { get; private set; }

    internal void SetActiveUser(Customer user)
    {
        ActiveUser = user;
    }

    internal void CreateNewAccount(AccountType type, Currency currency)
    {
        BankAccount account = type switch
        {
            AccountType.Personal => new PersonalAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Personal },
            AccountType.Savings => new SavingsAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Savings },
            AccountType.Loan => new LoanAccount { AccountNumber = _repository.GetNextAccountNumber().ToString(), accountType = AccountType.Loan },
            _ => throw new InvalidOperationException()
        };

        account.OwnerId = ActiveUser.Id;
        account.Currency = currency;

        _repository.Add(account);
    }

    /* LEFT TO DO:
     * 1. Make sure currency conversion is taken into consideration.
     * 2. Make sure converted amounts are rounded.
     * 3. Make sure transaction history works for loan accounts!
     * 4. Look through the comments...
     * 5. Test!
     */
    
    public bool TransferMoney(BankAccount sourceAccount, BankAccount destinationAccount, decimal amount)
    {
        if (amount <= 0)
        {
            return false;
        }
        
        if (ReferenceEquals(sourceAccount, destinationAccount))
        {
            // TODO: specific error: same source and destination account
            return false;
        }

        if (!_exchangeRateService.TryGetRate(sourceAccount.Currency, destinationAccount.Currency, out decimal exchangeRate))
        {
            return false;
        }
        
        // I think this will work...?
        if (sourceAccount is LoanAccount loanSource)
        {
            if (loanSource.Owed <= 0)
            {
                // TODO: Display error message:
                Console.WriteLine("You don't have a loan!"); // TEMP
                return false;
            }

            if (amount > loanSource.Owed)
            {
                // TODO: Display error message:
                Console.WriteLine("Your loan isn't big enough for that transfer!"); // TEMP
                return false;
            }

            // Nothing is done with LoanAccount.Owed since the loan remains.
            loanSource.AmountWithdrawn += amount;

            if (amount > (loanSource.Owed - loanSource.AmountWithdrawn))
            {
                // TODO: Display error message:
                Console.WriteLine("Your loan isn't big enough for that transfer!"); // TEMP

                loanSource.AmountWithdrawn -= amount; // reset tracker

                return false;
            }
        }
        
        if (sourceAccount is PersonalAccount personalSource) // also covers savings account
        {
            if (personalSource.Balance < amount)
            {
                // TODO: display error:
                Console.WriteLine("Insufficient funds"); // TEMP
                return false;
            }
            
            personalSource.Balance -= amount;
        }

        if (sourceAccount is LoanAccount && destinationAccount is LoanAccount)
        {
            // TODO: add error message: (is this necessary?)
            Console.WriteLine("Both accounts can't be loan accounts!"); // TEMP
        }
        
        decimal destinationAmount = amount * exchangeRate;
        bool hasConverted = sourceAccount.Currency != destinationAccount.Currency;
        
        if (destinationAccount is LoanAccount loanDestination)
        {
            if (destinationAmount > loanDestination.Owed)
            {
                // TODO: display error message
                Console.WriteLine($"Maximum amount available to transfer into loan account equals the amount owed: {loanDestination.Owed} {loanDestination.Currency}."); // TEMP
                return false;
            }
            
            loanDestination.Owed -= destinationAmount; // This is already converted into correct currency

            if (loanDestination.Owed == 0)
            {
                loanDestination.AmountWithdrawn = 0;
                // TODO: display information:
                Console.WriteLine("Your loan was successfully payed off!"); // TEMP
            }

            if (destinationAmount < loanDestination.AmountWithdrawn)
            {
                loanDestination.AmountWithdrawn -= destinationAmount;
                // TODO: display information:
                Console.WriteLine($"Your withdrawn amount was decreased with {destinationAmount} {loanDestination.Currency}");
            }
        }

        if (destinationAccount is PersonalAccount personalDestination)
        {
            personalDestination.Balance += destinationAmount;
        }

        // Add correct transaction history for loan accounts!

        sourceAccount.TransactionHistory.Add(new Transaction
        {
            Type = TransactionType.Transfer,
            Amount = -amount,
            Currency = sourceAccount.Currency,
            ConvertedAmount = hasConverted ? destinationAmount : null,
            ConvertedCurrency = hasConverted ? destinationAccount.Currency : null,
            ExchangeRate = hasConverted ? exchangeRate : null,
            RelatedAccountId = destinationAccount.AccountNumber,
            Desciption = $"Transfer to {destinationAccount.AccountNumber}"
        });

        destinationAccount.TransactionHistory.Add(new Transaction
        {
            Type = TransactionType.Transfer,
            Amount = destinationAmount,
            Currency = destinationAccount.Currency,
            ConvertedAmount = hasConverted ? amount : null,
            ConvertedCurrency = hasConverted ? sourceAccount.Currency : null,
            ExchangeRate = hasConverted ? 1 / exchangeRate : null,
            RelatedAccountId = sourceAccount.AccountNumber,
            Desciption = $"Transfer from {sourceAccount.AccountNumber}"
        });

        return true;
    }

    public decimal CalculateExpectedInterest(SavingsAccount savingsAccount, decimal depositAmount, int years)
    {
        if (depositAmount <= 0 || years <= 0)
        {
            return 0m;
        }

        decimal interestRate = savingsAccount.InterestRate;
        decimal balance = depositAmount;

        for (int year = 1; year <= years; year++)
        {
            balance += balance * interestRate;
        }

        return balance - depositAmount;
    }

    public decimal CalculateLoanInterest(LoanAccount loanAccount, decimal loanAmount, int years)
    {
        if (loanAmount <= 0 || years <= 0 || loanAccount.InterestRate < 0)
        {
            return 0m;
        }

        decimal interestRate = loanAccount.InterestRate;

        return loanAmount * interestRate * years;
    }

    public bool BorrowMoney(LoanAccount loanAccount, decimal loanAmount, int years)
    {
        if (loanAmount <= 0 || years <= 0 || loanAccount.InterestRate < 0)
        {
            return false;
        }

        decimal totalInterest = CalculateLoanInterest(loanAccount, loanAmount, years);

        decimal maxLoanAmount = CalculateMaxLoanAmount(loanAccount);

        decimal newTotalDebt = loanAccount.Owed + loanAmount + totalInterest;

        if (newTotalDebt > maxLoanAmount)
        {
            return false;
        }

        loanAccount.Owed += loanAmount + totalInterest;

        return true;
    }

    public decimal CalculateMaxLoanAmount(LoanAccount loanAccount)
    {
        List<BankAccount>? customerAccounts =
             _repository.GetBankAccountsOwnedByCustomer(ActiveUser);

        if (customerAccounts is null)
        {
            return 0m;
        }

        List<PersonalAccount> balanceAccounts = customerAccounts.OfType<PersonalAccount>().ToList();

        decimal totalBalance = 0m;

        foreach (PersonalAccount account in balanceAccounts)
        {
            if (!_exchangeRateService.TryGetRate(account.Currency, loanAccount.Currency, out decimal exchangeRate))
            {
                throw new InvalidOperationException("Currency conversion failed.");
            }

            decimal convertedBalance = account.Balance * exchangeRate;

            totalBalance += convertedBalance;
        }

        return totalBalance * 5;
    }
}

