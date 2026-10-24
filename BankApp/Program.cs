using BankApp.Controllers;
using BankApp.Models;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

var userRepository = new UserRepository();
var errorMessageView = new ErrorMessageView();
var authenticationService = new AuthenticationService(userRepository);
var authenticationView = new AuthenticationView();
var authenticationController = new AuthenticationController(authenticationService, authenticationView, errorMessageView);

var user = authenticationController.Run();

var accountRepository = new AccountRepository();
var exchangeRateService = new ExchangeRateService();

var bankService = new BankService(accountRepository, exchangeRateService);
var bankView = new BankView();
var bankController = new BankController(bankService, bankView, accountRepository, errorMessageView);

if (user is Customer customer)
{
    bankController.Run(customer);
}