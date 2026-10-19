using BankApp.Controllers;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

var accountRepository = new AccountRepository();
var bankService = new BankService(accountRepository);
var bankView = new BankView();
var bankController = new BankController(bankService, bankView);