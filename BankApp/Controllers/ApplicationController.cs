using BankApp.Models;

namespace BankApp.Controllers;

internal class ApplicationController(AuthenticationController authentication, BankController bank, AdminController admin)
{
    private readonly AuthenticationController _authentication = authentication;
    private readonly BankController _bank = bank;
    private readonly AdminController _admin = admin;

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
                _admin.Run(user);
            }
        }
    }
}
