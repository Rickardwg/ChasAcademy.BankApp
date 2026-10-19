using BankApp.Models;
using BankApp.Services;
using BankApp.Views;

namespace BankApp.Controllers;

internal class BankController(BankService service, BankView view)
{
    private readonly BankService _service = service;
    private readonly BankView _view = view;

    internal void Run(User user)
    {

    }
}