using BankApp.Controllers;
using BankApp.DependencyInjection;
using BankApp.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddBankAppServices();

using var provider = services.BuildServiceProvider();

var scheduler = provider.GetRequiredService<TransactionSchedulingService>();
using var cancellationTokenSource = new CancellationTokenSource();
Task schedulerTask = scheduler.RunAsync(cancellationTokenSource.Token);

provider.GetRequiredService<ApplicationController>().Run();

cancellationTokenSource.Cancel();
await schedulerTask;