using BankApp.Controllers;
using BankApp.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddBankAppServices();

using var provider = services.BuildServiceProvider();
provider.GetRequiredService<ApplicationController>().Run();