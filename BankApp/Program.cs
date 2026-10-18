using BankApp.Controllers;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;

var userRepository = new UserRepository();
var authenticationService = new AuthenticationService(userRepository);
var authenticationView = new AuthenticationView();
var authenticationController = new AuthenticationController(authenticationService, authenticationView);
