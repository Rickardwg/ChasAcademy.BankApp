using BankApp.Controllers;
using BankApp.Repositories;
using BankApp.Services;
using BankApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BankApp.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankAppServices(this IServiceCollection services)
    {
        services.AddSingleton<UserRepository>();
        services.AddSingleton<AccountRepository>();
        services.AddSingleton<ExchangeRateService>();

        services.AddTransient<AuthenticationService>();
        services.AddTransient<BankService>();

        services.AddTransient<AuthenticationController>();
        services.AddTransient<BankController>();
        services.AddTransient<ApplicationController>();

        services.AddTransient<AuthenticationView>();
        services.AddTransient<BankView>();
        services.AddTransient<ErrorMessageView>();

        return services;
    }
}