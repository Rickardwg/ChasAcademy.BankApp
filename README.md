# BankApp

BankApp is a console-based banking application developed in C# and .NET. It provides separate flows for customers and administrators, and includes functionality for account management, money transfers, authentication, currency conversion, and loan calculations.

The application uses [Spectre.Console](https://spectreconsole.net/) to create interactive terminal menus, tables, and panels. Its source code is divided into separate layers so that user interaction, application flow, business logic, and data storage have distinct responsibilities.

## Source Code Structure

The solution contains one project, `BankApp`, organized into the following folders and files:

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
│   │   ├── PendingTransaction.cs
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
│   │   ├── ExchangeRateService.cs
│   │   └── TransactionSchedulingService.cs
│   ├── Views/
│   │   ├── AdminView.cs
│   │   ├── AuthenticationView.cs
│   │   ├── BankView.cs
│   │   ├── ConsoleHeader.cs
│   │   └── ErrorMessageView.cs
│   ├── BankApp.csproj
│   └── Program.cs
├── .gitattributes
├── .gitignore
├── BankApp.slnx
└── README.md
```

## UML Diagram

The UML class diagram provides an overview of the application's
classes, properties, inheritance relationships, and dependencies
between the different layers.

![BankApp UML Class Diagram](./BankApp/docs/diagrams/UML-dark.png)

### Program Entry Point

- **`Program.cs`** – The application's entry point. It creates the dependency injection service collection, registers the application components through `AddBankAppServices()`, and builds the service provider. It then starts `TransactionSchedulingService.RunAsync()` as a background task before starting `ApplicationController.Run()`. When the application flow exits, it cancels the scheduler by using a `CancellationTokenSource` and awaits the scheduler task.
- **`BankApp.csproj`** – Defines the project configuration, target framework, nullable reference type settings, and NuGet package dependencies.
- **`BankApp.slnx`** – The solution file that references the `BankApp` project.

### Common

- **`Result<TValue, TError>` (`Common/Results/Result.cs`)** – A generic result object used by services to report whether an operation succeeded or failed. It contains an `IsSuccess` flag and either a returned `Value` or an `Error`. This lets controllers handle expected failures without relying on exceptions for normal validation errors.

## Controllers

Controllers coordinate the application flow. They receive input from views, call the relevant services or repositories, and decide what should happen next.

- **`ApplicationController`** – Coordinates the overall application flow. It starts authentication and directs the signed-in user to the customer banking area or the administrator area depending on the user's type and role.
- **`AuthenticationController`** – Controls the login flow. It asks `AuthenticationView` for credentials, calls `AuthenticationService` to verify them, and handles invalid credentials and the failed-login limit.
- **`BankController`** – Controls the customer banking menu. It handles account listing, account creation, transfers between the customer's own accounts, transfers to other customers, and the loan flow. It uses `BankService` for banking operations and `AccountRepository` to retrieve accounts. For both types of transfers, it creates a `PendingTransaction` and places it in `TransactionSchedulingService`'s queue instead of executing the transfer immediately. It then informs the customer that the transfer has been scheduled.
- **`AdminController`** – Controls the administrator area and user-management menu. It handles registration of new customer and administrator accounts, including displaying validation errors and allowing the administrator to correct invalid input. It also coordinates currency-rate updates by retrieving current rates from `ExchangeRateService`, asking the administrator to select a currency, and submitting the new rate for validation.

## DependencyInjection

- **`ServiceCollectionExtensions` (`DependencyInjection/ServiceCollectionExtensions.cs`)** – Contains the `AddBankAppServices()` extension method. It registers repositories, services, controllers, views, and the password hasher with Microsoft's dependency injection container. `UserRepository`, `AccountRepository`, `ExchangeRateService`, and `TransactionSchedulingService` are registered as singletons. The other application components, including controllers, views, and the main business services, are registered as transients.

## Enums

Enums provide named values for options and error states used throughout the application.

- **`AccountType`** – Defines the supported account types: `Personal`, `Savings`, and `Loan`.
- **`Currency`** – Defines the supported currencies: `SEK`, `EUR`, and `USD`.
- **`TransactionType`** – Defines transaction categories: `Deposit`, `Withdrawal`, and `Transfer`.
- **`ErrorMessages`** – Defines general application and banking errors that can be displayed by `ErrorMessageView`, such as invalid menu choices, missing accounts, and invalid transfers.
- **`LoginError`** – Defines authentication failures, including invalid credentials and the failed-login limit being reached.
- **`RegistrationError`** – Defines user-registration validation errors, such as an existing username or invalid personal details.

## Models

Models represent the main data objects used by the banking application. They describe users, bank accounts, transactions, and transfers waiting to be processed.

- **`User`** – The base user model. It contains the user's ID, username, password hash, and administrator flag (`IsAdmin`).
- **`Customer`** – Inherits from `User` and adds customer details such as first name, last name, email address, and phone number.
- **`BankAccount`** – The base class for bank accounts. It contains the owner's ID, account number, currency, account type, and transaction history.
- **`PersonalAccount`** – Inherits from `BankAccount` and adds a balance. It represents a regular personal bank account.
- **`SavingsAccount`** – Inherits from `PersonalAccount` and adds an interest rate. It therefore has the balance functionality of a personal account as well as savings-specific interest information.
- **`LoanAccount`** – Inherits from `BankAccount` and contains the amount owed, the loan interest rate, and `AmountWithdrawn`, which tracks how much of the borrowed principal has been withdrawn from the loan account.
- **`Transaction`** – Represents a completed transaction recorded in an account's transaction history. It stores the date, transaction type, amount, currency, and optional information about converted amounts, exchange rates, related accounts, and a description.
- **`PendingTransaction`** – Represents a transfer waiting in the scheduler's queue. It stores the source account, destination account, transfer amount, and the time when the pending transaction was created (`CreatedAt`). The scheduler later passes the transfer to `BankService` for processing.

The main inheritance relationships are:

```text
User
└── Customer

BankAccount
├── PersonalAccount
│   └── SavingsAccount
└── LoanAccount
```

## Repositories

Repositories are responsible for storing and retrieving application data. The current implementation uses in-memory lists rather than a database.

- **`UserRepository`** – Stores users, provides lookup by username, and adds newly registered users. It also initializes the application with example customer and administrator accounts.
- **`AccountRepository`** – Stores bank accounts, generates the next account number, and retrieves accounts belonging to a specific customer or to other customers.

Because the repositories currently store data in memory, users and account changes are not persisted when the application is closed. The initial example users are created again when the application starts.

## Services

Services contain the application's business logic and background processing. Controllers call services to perform operations rather than implementing the banking rules themselves.

- **`AuthenticationService`** – Finds users through `UserRepository`, verifies password hashes using `IPasswordHasher<User>`, handles password rehashing when required, and limits consecutive failed login attempts.
- **`AdminService`** – Registers customer and administrator users. It checks usernames and validates customer information, including email addresses, Swedish phone numbers, and password and username lengths. Passwords are stored as hashes.
- **`BankService`** – Contains the main banking operations. It creates accounts, processes transfers, records completed transfer transactions, calculates savings interest and loan interest, processes loan requests, and calculates a customer's maximum loan amount. Transfers involving the customer's own loan account support withdrawing available borrowed funds and making repayments to reduce the outstanding debt. Loan withdrawals are checked against the available borrowed principal, and loan repayments cannot exceed the outstanding debt. Currency conversion is applied when the source and destination accounts use different currencies.
- **`ExchangeRateService`** – Stores exchange rates relative to SEK and provides methods to calculate conversion rates and update supported currency rates. `GetRatesInSek()` provides the current non-SEK rates for the administrator interface, while `TryUpdateRate()` validates and updates a rate. SEK remains the fixed base currency, and rates are held in memory.
- **`TransactionSchedulingService`** – Manages a queue of `PendingTransaction` objects. `EnqueueTransaction()` adds a pending transfer to the queue and `DequeueTransaction()` retrieves the next one. A lock protects queue operations. Its asynchronous `RunAsync()` method waits until the next quarter-hour boundary (for example, 10:15, 10:30, 10:45, or 11:00) and then processes the queued transfers by calling `BankService.TransferMoney()`. It continues running until its cancellation token is signalled.

## Views

Views are responsible for presenting information and collecting input from the user. The application uses Spectre.Console for interactive prompts and terminal output.

- **`AuthenticationView`** – Displays the login menu and reads the username and password. The password prompt hides the entered text.
- **`BankView`** – Displays the customer banking menu, account tables, account-selection prompts, transfer prompts and results, transaction history, and loan-related information and previews. Loan-account rows also show the account number, amount owed, and amount withdrawn. `DisplayTransferScheduled()` informs the customer that a transfer has been queued for processing at the next quarter-hour. `WaitForContinue()` pauses the interface between operations so users have time to read messages and results.
- **`AdminView`** – Displays the administrator menus and collects information when registering customers or administrators. It also displays registration success and validation messages. For currency management, it displays the available currencies with their current SEK rates, collects a replacement rate, and shows whether the update succeeded.
- **`ErrorMessageView`** – Displays application errors in formatted terminal panels based on the `ErrorMessages` enum.
- **`ConsoleHeader`** – Clears the terminal and displays the application's title/logo.

## Recent Updates

The latest source-code changes introduce scheduled transfers:

- **Scheduled transfers** – Transfers between a customer's own accounts and transfers to another customer's account are added to a queue as `PendingTransaction` objects. The customer is shown a scheduled-transfer message instead of an immediate transfer result.
- **Quarter-hour processing** – `TransactionSchedulingService` checks the queue at the next quarter-hour boundary and processes queued transfers through `BankService.TransferMoney()`.
- **Background scheduler lifecycle** – `Program.cs` starts the scheduler as an asynchronous background task while the application is running. When the application exits, it signals cancellation and waits for the scheduler task to stop.
- **Thread-safe queue access** – A lock protects adding and removing pending transactions from the queue.
- **Dependency injection registration** – `TransactionSchedulingService` is registered as a singleton so that the customer controller and background task use the same queue instance.

## Application Flow

1. `Program.cs` configures dependency injection and starts `TransactionSchedulingService.RunAsync()` in the background.
2. `Program.cs` starts `ApplicationController`.
3. `ApplicationController` calls `AuthenticationController` to display the login menu.
4. `AuthenticationController` uses `AuthenticationService` to authenticate the user.
5. After a successful login, `ApplicationController` directs customers to `BankController` and administrators to `AdminController`.
6. The relevant controller collects input through its view and calls services or repositories to perform the requested operation.
7. When a customer requests a transfer, `BankController` creates a `PendingTransaction` and adds it to `TransactionSchedulingService`.
8. At the next quarter-hour boundary, the scheduler dequeues pending transfers and asks `BankService` to process them. `BankService` applies the banking rules, currency conversion, and transaction recording.
9. When the application exits, `Program.cs` cancels the scheduler task and awaits its completion.

This separation keeps the console interface, application flow, business rules, data access, and scheduled processing in distinct parts of the source code.

## Technologies

- **C# / .NET 10** – Main programming language and application framework.
- **Spectre.Console** – Interactive console menus, prompts, tables, and panels.
- **Microsoft.Extensions.DependencyInjection** – Dependency injection and component registration.
- **Microsoft.AspNetCore.Identity** – Password hashing and password verification through `IPasswordHasher<User>`.
- **.NET asynchronous programming** – `Task`, `CancellationToken`, and `CancellationTokenSource` are used to run and stop the transfer scheduler.

## Run the Application

The .NET 10 SDK is required.

```bash
git clone https://github.com/Rickardwg/ChasAcademy.BankApp.git
cd ChasAcademy.BankApp
dotnet run --project BankApp/BankApp.csproj
```

NuGet dependencies are restored automatically when the project is built or run.

## Current Scope

The project includes customer authentication, account creation and listing, transfers between a customer's own accounts and to other customers, currency conversion, and loan calculations. Customers can use transfers between their own accounts to withdraw available funds from a loan account or repay an outstanding loan. Customer transfers are scheduled and processed by the background scheduler at the next quarter-hour boundary rather than being executed immediately. The administrator flow supports adding users and updating supported currency rates (EUR and USD); SEK remains the fixed base currency. Editing and removing users are not currently implemented.

The repositories, exchange rates, and pending-transfer queue use in-memory storage. As a result, user, account, transaction, exchange-rate, and queued-transfer data are not persisted when the application is closed. A pending transfer that has not yet reached its scheduled processing time is therefore lost if the application shuts down first. The scheduler currently calls `BankService.TransferMoney()` but does not use its returned success/failure value to notify the customer after processing; the confirmation shown in the console only means that the transfer was queued.
