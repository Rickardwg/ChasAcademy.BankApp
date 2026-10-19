using BankApp.Controllers;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

var userRepository = new UserRepository();
var authenticationService = new AuthenticationService(userRepository);
var authenticationView = new AuthenticationView();
var authenticationController = new AuthenticationController(authenticationService, authenticationView);

var user = authenticationController.Run();

var accountRepository = new AccountRepository();
var bankService = new BankService(accountRepository);
var bankView = new BankView();
var bankController = new BankController(bankService, bankView);