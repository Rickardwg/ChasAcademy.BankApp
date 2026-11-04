# BankApp

BankApp is a console-based banking application developed in C# and .NET. It provides separate flows for customers and administrators, and includes functionality for account management, money transfers, authentication, currency conversion, and loan calculations.

The application uses [Spectre.Console](https://spectreconsole.net/) to create interactive terminal menus, tables, and panels. Its source code is divided into separate layers so that user interaction, application flow, business logic, and data storage have distinct responsibilities.

## Source Code Structure

The solution contains one project, `BankApp`, which is organized into the following folders and files:

```text
ChasAcademy.BankApp/
├── .github/
│   └── pull_request_template.md
├── BankApp/
│   ├── Common/
│   │   └── Results/
│   │       └── Result.cs
│   ├── Controllers/
│   │   ├── AdminController.cs
│   │   ├── ApplicationController.cs
│   │   ├── AuthenticationController.cs
│   │   └── BankController.cs
│   ├── DependencyInjection/
│   │   └── ServiceCollectionExtensions.cs
│   ├── Enums/
│   │   ├── AccountType.cs
│   │   ├── Currency.cs
│   │   ├── ErrorMessages.cs
│   │   ├── TransactionType.cs
│   │   └── Errors/
│   │       ├── LoginError.cs
│   │       └── RegistrationError.cs
│   ├── Models/
│   │   ├── BankAccount.cs
│   │   ├── Customer.cs
│   │   ├── LoanAccount.cs
│   │   ├── PersonalAccount.cs
│   │   ├── SavingsAccount.cs
│   │   ├── Transaction.cs
│   │   └── User.cs
│   ├── Repositories/
│   │   ├── AccountRepository.cs
│   │   └── UserRepository.cs
│   ├── Services/
│   │   ├── AdminService.cs
│   │   ├── AuthenticationService.cs
│   │   ├── BankService.cs
│   │   └── ExchangeRateService.cs
│   ├── Views/
│   │   ├── AdminView.cs
│   │   ├── AuthenticationView.cs
│   │   ├── BankView.cs
│   │   ├── ConsoleHeader.cs
│   │   └── ErrorMessageView.cs
│   ├── BankApp.csproj
│   └── Program.cs
├── BankApp.slnx
└── README.md
```

### Program Entry Point

- **`Program.cs`** – The application's entry point. It creates a dependency injection service collection, registers the application services by calling `AddBankAppServices()`, builds the service provider, and starts the application through `ApplicationController.Run()`.
- **`BankApp.csproj`** – Defines the project configuration, target framework, nullable reference type settings, and NuGet package dependencies.
- **`BankApp.slnx`** – The solution file that references the `BankApp` project.

### Common

- **`Result<TValue, TError>` (`Common/Results/Result.cs`)** – A generic result object used by services to report whether an operation succeeded or failed. It contains an `IsSuccess` flag and either a returned `Value` or an `Error`. This lets controllers handle expected failures without relying on exceptions for normal validation errors.
