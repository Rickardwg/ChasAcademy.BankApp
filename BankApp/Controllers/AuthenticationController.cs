using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class AuthenticationController(AuthenticationService service, AuthenticationView view)
{
    private readonly AuthenticationService _service = service;
    private readonly AuthenticationView _view = view;
}
