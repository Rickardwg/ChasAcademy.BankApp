using BankApp.Models;
using BankApp.Views;

Console.WriteLine("Hello, World!");
PersonalAccount testAccount = new PersonalAccount
{
    AccountNumber = "123454789",
    Balance = 500000m
};

List<BankAccount> testAccounts = new List<BankAccount>();

testAccounts.Add(testAccount);


SavingsAccount testSaveAcc = new SavingsAccount
{
    InterestRate = 0.10m,
    Balance = 40000m,
    AccountNumber = "123456"
};
testAccounts.Add(testSaveAcc);

LoanAccount testLoanAccount = new LoanAccount
{
    Owed = 10000.0000000m
};
testAccounts.Add(testLoanAccount);

BankView bankView = new BankView();
bankView.DisplayAccounts(testAccounts);
// TODO: 

// SavingsAccount skapade object min inte lagt till det till testAccount
// Gör: Lägg till SavingsAccount till listan för att see output
// Testa LoanAccount och Owed property.
// Tillsist, review BankView och tabort test kod, sedan commita.