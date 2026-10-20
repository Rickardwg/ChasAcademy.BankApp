using BankApp.Repositories;

namespace BankApp.Services;

internal class BankService(AccountRepository repository)
{
    private readonly AccountRepository _repository = repository;
}
