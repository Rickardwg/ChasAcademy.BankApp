using BankApp.Models;
using BankApp.Repositories;

namespace BankApp.Services;

internal class AuthenticationService(UserRepository repository)
{
    private readonly UserRepository _repository = repository;
}
