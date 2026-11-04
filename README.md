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

### Controllers

Controllers coordinate the application flow. They receive input from views, call the relevant services or repositories, and decide what should happen next.

- **`ApplicationController`** – Coordinates the overall application flow. It starts authentication and directs the signed-in user to the customer banking area or the administrator area depending on the user's type and role.
- **`AuthenticationController`** – Controls the login flow. It asks the authentication view for credentials, calls `AuthenticationService` to verify them, and handles invalid credentials and the failed-login limit.
- **`BankController`** – Controls the customer banking menu. It handles account listing, transfers between the customer's own accounts, transfers to other customers, account creation, and the loan flow. It uses `BankService` for banking operations and `AccountRepository` to retrieve accounts.
- **`AdminController`** – Controls the administrator area and user-management menu. It handles registration of new customer and administrator accounts, including displaying validation errors and allowing the administrator to correct invalid input.

### DependencyInjection

- **`ServiceCollectionExtensions` (`DependencyInjection/ServiceCollectionExtensions.cs`)** – Contains the `AddBankAppServices()` extension method. It registers repositories, services, controllers, views, and the password hasher with Microsoft's dependency injection container. Repositories and the exchange-rate service are registered as singletons, while the other application components are registered as transients.

### Enums

Enums provide named values for options and error states used throughout the application.

- **`AccountType`** – Defines the supported account types: `Personal`, `Savings`, and `Loan`.
- **`Currency`** – Defines the supported currencies: `SEK`, `EUR`, and `USD`.
- **`TransactionType`** – Defines transaction categories: `Deposit`, `Withdrawal`, and `Transfer`.
- **`ErrorMessages`** – Defines general application and banking errors that can be displayed by `ErrorMessageView`, such as invalid menu choices, missing accounts, and invalid transfers.
- **`LoginError`** – Defines authentication failures, including invalid credentials and the failed-login limit being reached.
- **`RegistrationError`** – Defines user-registration validation errors, such as an existing username or invalid personal details.

### Models

Models represent the main data objects used by the banking application. They describe users, bank accounts, and transactions.

- **`User`** – The base user model. It contains the user's ID, username, password hash, and administrator flag (`IsAdmin`).
- **`Customer`** – Inherits from `User` and adds customer details such as first name, last name, email address, and phone number.
- **`BankAccount`** – The base class for bank accounts. It contains the owner's ID, account number, currency, account type, and transaction history.
- **`PersonalAccount`** – Inherits from `BankAccount` and adds a balance. It represents a regular personal bank account.
- **`SavingsAccount`** – Inherits from `PersonalAccount` and adds an interest rate. It therefore has the balance functionality of a personal account as well as savings-specific interest information.
- **`LoanAccount`** – Inherits from `BankAccount` and contains the amount owed and the loan interest rate.
- **`Transaction`** – Represents a transaction recorded in an account's transaction history. It stores the date, transaction type, amount, currency, and optional information about converted amounts, exchange rates, related accounts, and a description.

The main inheritance relationships are:

```text
User
└── Customer

BankAccount
├── PersonalAccount
│   └── SavingsAccount
└── LoanAccount
```

### Repositories

Repositories are responsible for storing and retrieving application data. The current implementation uses in-memory lists rather than a database.

- **`UserRepository`** – Stores users, provides lookup by username, and adds newly registered users. It also initializes the application with example customer and administrator accounts.
- **`AccountRepository`** – Stores bank accounts, generates the next account number, and retrieves accounts belonging to a specific customer or to other customers.

Because the repositories currently store data in memory, users and account changes are not persisted when the application is closed. The initial example users are created again when the application starts.

### Services

Services contain the application's business logic. Controllers call services to perform operations rather than implementing the banking rules themselves.

- **`AuthenticationService`** – Finds users through `UserRepository`, verifies password hashes using `IPasswordHasher<User>`, handles password rehashing when required, and limits consecutive failed login attempts.
- **`AdminService`** – Registers customer and administrator users. It checks usernames and validates customer information, including email addresses, Swedish phone numbers, and password and username lengths. Passwords are stored as hashes.
- **`BankService`** – Contains the main banking operations. It creates accounts, transfers money, records transfer transactions, calculates savings interest and loan interest, processes loan requests, and calculates a customer's maximum loan amount.
- **`ExchangeRateService`** – Stores exchange rates relative to SEK and provides methods to calculate conversion rates and update supported currency rates. The initial rates are held in memory.

### Views

Views are responsible for presenting information and collecting input from the user. The application uses Spectre.Console for interactive prompts and terminal output.

- **`AuthenticationView`** – Displays the login menu and reads the username and password. The password prompt hides the entered text.
- **`BankView`** – Displays the customer banking menu, account tables, account-selection prompts, transfer prompts and results, transaction history, and loan-related information and previews.
- **`AdminView`** – Displays the administrator menus and collects information when registering customers or administrators. It also displays registration success and validation messages.
- **`ErrorMessageView`** – Displays application errors in formatted terminal panels based on the `ErrorMessages` enum.
- **`ConsoleHeader`** – Clears the terminal and displays the application's title/logo.

## Application Flow

1. `Program.cs` configures dependency injection and starts `ApplicationController`.
2. `ApplicationController` calls `AuthenticationController` to display the login menu.
3. `AuthenticationController` uses `AuthenticationService` to authenticate the user.
4. After a successful login, `ApplicationController` directs customers to `BankController` and administrators to `AdminController`.
5. The relevant controller collects input through its view and calls services or repositories to perform the requested operation.
6. The views display the result or any applicable error message to the user.

This separation keeps the console interface, application flow, business rules, and data access in distinct parts of the source code.

## Technologies

- **C# / .NET 10** – Main programming language and application framework.
- **Spectre.Console** – Interactive console menus, prompts, tables, and panels.
- **Microsoft.Extensions.DependencyInjection** – Dependency injection and component registration.
- **Microsoft.AspNetCore.Identity** – Password hashing and password verification through `IPasswordHasher<User>`.

## Run the Application

The .NET 10 SDK is required.

```bash
git clone https://github.com/Rickardwg/ChasAcademy.BankApp.git
cd ChasAcademy.BankApp
dotnet run --project BankApp/BankApp.csproj
```

NuGet dependencies are restored automatically when the project is built or run.

## Current Scope

The project includes customer authentication, account creation and listing, transfers, currency conversion, and loan calculations. The administrator flow currently supports adding users. Some menu options, such as editing or removing users and managing currency settings, are present in the menus but do not yet have implemented actions. The repositories also use in-memory storage, so the application does not currently provide persistent database storage.
