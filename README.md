
# Expense Reimbursement Management System

## 1. Project Overview

The Expense Reimbursement Management System is a C# application designed to manage employee expense claims. It allows users to submit expense claims, view claim details, track claim statuses, and generate expense summaries.

The project uses an ASP.NET Core MVC web application, an ASP.NET Core Web API, SQL Server, and a C# Console application.

## 2. Technologies Used

- C#
- .NET 10
- ASP.NET Core MVC
- ASP.NET Core Web API
- Razor Views
- HTML and CSS
- Bootstrap
- SQL Server
- Entity Framework Core
- HttpClient
- Visual Studio Code
- SQL Server Management Studio (SSMS)

## 3. Project Architecture

The application follows this architecture:

Browser → MVC Web Application → Web API → SQL Server

Console Application → Web API → SQL Server

The MVC application and Console application communicate with the Web API using HttpClient. Neither application accesses SQL Server directly.

## 4. Project Structure

```text
ExpenseReimbursementSystem/
│
├── ExpenseManagement.API/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   └── Program.cs
│
├── ExpenseManagement.Web/
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   │   ├── Expenses/
│   │   └── Shared/
│   ├── appsettings.json
│   └── Program.cs
│
├── ExpenseManagement.Console/
│   └── Program.cs
│
└── README.md
```

## 5. Main Features

### Dashboard
- Displays the expense claim summary.
- Shows the latest five expense claims.
- Provides links to view individual claims and the complete claim list.

### Create Expense Claim
- Accepts employee details and expense information.
- Validates claim inputs.
- Submits new claims through the API.
- New claims receive Pending status.

### Expense List
- Displays expense claims.
- Supports searching and status filtering.
- Allows permitted claim status changes.

### Claim Details
- Displays the complete details of a selected claim.
- Handles claims that cannot be found.

### Console Application
- Generates an expense summary.
- Displays Pending claims.
- Searches for a claim by Claim ID.
- Handles invalid input and API errors.

## 6. Database

Database name: `ExpenseDB`

Table name: `dbo.Expenses`

The table stores:
- ClaimId
- EmployeeId
- EmployeeName
- Category
- Amount
- ExpenseDate
- Description
- Status
- CreatedAt

Claim IDs are generated automatically. New claims are stored with Pending status.

## 7. Expense Status Rules

The permitted status transitions are:

- Pending → Approved
- Pending → Rejected
- Approved → Paid

Other status transitions are rejected by the API.

## 8. API Endpoints

Base URL: `http://localhost:5256`

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/expenses` | Retrieve all expense claims |
| GET | `/api/expenses/{id}` | Retrieve a claim by ID |
| POST | `/api/expenses` | Create an expense claim |
| GET | `/api/expenses/employee/{employeeId}` | Retrieve claims for an employee |
| PUT | `/api/expenses/{id}/status` | Update a claim status |
| GET | `/api/expenses/summary` | Retrieve expense summary data |

Swagger documentation is available at:

`http://localhost:5256/swagger`

## 9. Prerequisites

Install or configure:

- .NET 10 SDK
- SQL Server
- SQL Server Management Studio
- Visual Studio Code

Ensure the `ExpenseDB` database and `dbo.Expenses` table exist before running the application.

## 10. Configuration

The API connection string should point to the local SQL Server instance and `ExpenseDB`.

Example:

```text
Server=localhost;Database=ExpenseDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

The MVC application's API base URL should be:

```text
http://localhost:5256/
```

The Console application also communicates with the API at:

```text
http://localhost:5256/
```

Adjust the connection string if your SQL Server instance uses a different server name.

## 11. How to Run the Project

Open a terminal in the solution root folder:

`C:\Users\pavip\ExpenseReimbursementSystem`

### Terminal 1: Run the API

```powershell
dotnet run --project ExpenseManagement.API --urls "http://localhost:5256"
```

### Terminal 2: Run the MVC Website

```powershell
dotnet run --project ExpenseManagement.Web --urls "http://localhost:5045"
```

Open the Dashboard:

`http://localhost:5045/Expenses/Dashboard`

### Terminal 3: Run the Console Application

```powershell
dotnet run --project ExpenseManagement.Console
```

Keep the API running while using the MVC website or Console application.

## 12. Build Commands

Build the API:

```powershell
dotnet build ExpenseManagement.API
```

Build the MVC website:

```powershell
dotnet build ExpenseManagement.Web
```

Build the Console application:

```powershell
dotnet build ExpenseManagement.Console
```

## 13. Testing

The following functionality has been manually tested:

- Dashboard displays expense summary and latest claims.
- Expense List displays claims.
- Search by Employee ID works.
- Status filtering works.
- Reset Filters restores the list.
- Claim Details page opens for an existing claim.
- Missing Claim ID displays a not-found message.
- Console generates the expense summary.
- Console displays Pending claims.
- Console searches for a claim by ID.
- Console handles a nonexistent Claim ID.
- Console exits normally.
- SQL Server records and summary totals were verified in SSMS.

Additional assessment test cases should be recorded in a separate test report with test ID, input/action, expected result, actual result, and Pass/Fail status.

## 14. Conclusion

The Expense Reimbursement Management System demonstrates an ASP.NET Core MVC application, REST API integration, SQL Server data storage, and a C# Console application communicating through HttpClient.
