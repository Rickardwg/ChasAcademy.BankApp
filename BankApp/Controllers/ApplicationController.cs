using BankApp.Models;

namespace BankApp.Controllers;

internal class ApplicationController(AuthenticationController authentication, BankController bank)
{
    private readonly AuthenticationController _authentication = authentication;
    private readonly BankController _bank = bank;

    public void Run()
    {
        while (true)
        {
            User? user = _authentication.Run();

            if (user is null)
            {
                break;
            }

            if (user is Customer customer)
            {
                _bank.Run(customer);
            }

            if (user.IsAdmin)
            {

            }
        }
    }
}
