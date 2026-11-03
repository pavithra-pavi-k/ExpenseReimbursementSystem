
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseManagement.ConsoleApp;

public class Expense
{
    public int ClaimId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ExpenseSummary
{
    public int TotalClaims { get; set; }
    public int PendingClaims { get; set; }
    public int ApprovedClaims { get; set; }
    public int RejectedClaims { get; set; }
    public int PaidClaims { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal RejectedAmount { get; set; }
    public decimal PaidAmount { get; set; }
}

public class Program
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:5256/")
    };

    public static async Task Main()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine("     EXPENSE MANAGEMENT REPORT");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Generate Expense Summary");
            Console.WriteLine("2. Display Pending Claims");
            Console.WriteLine("3. Search Claim by ID");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await GenerateSummaryAsync();
                    break;

                case "2":
                    await DisplayPendingClaimsAsync();
                    break;

                case "3":
                    await SearchClaimByIdAsync();
                    break;

                case "4":
                    running = false;
                    Console.WriteLine("Exiting Expense Management Report.");
                    continue;

                default:
                    Console.WriteLine("Invalid choice. Enter a number from 1 to 4.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }

    private static async Task GenerateSummaryAsync()
    {
        Console.Clear();
        Console.WriteLine("========== EXPENSE SUMMARY ==========");

        try
        {
            var summary = await Client.GetFromJsonAsync<ExpenseSummary>(
                "api/expenses/summary");

            if (summary == null)
            {
                Console.WriteLine("The API returned an empty summary.");
                return;
            }

            Console.WriteLine($"Total claims: {summary.TotalClaims}");
            Console.WriteLine($"Pending claims: {summary.PendingClaims}");
            Console.WriteLine($"Approved claims: {summary.ApprovedClaims}");
            Console.WriteLine($"Rejected claims: {summary.RejectedClaims}");
            Console.WriteLine($"Paid claims: {summary.PaidClaims}");
            Console.WriteLine($"Total approved amount: ₹{summary.ApprovedAmount:N2}");
            Console.WriteLine($"Total rejected amount: ₹{summary.RejectedAmount:N2}");
            Console.WriteLine($"Total paid amount: ₹{summary.PaidAmount:N2}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Unable to retrieve the summary: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The API request timed out.");
        }
        catch (JsonException)
        {
            Console.WriteLine("The API returned invalid summary data.");
        }
    }

    private static async Task DisplayPendingClaimsAsync()
    {
        Console.Clear();
        Console.WriteLine("========== PENDING EXPENSE CLAIMS ==========");

        try
        {
            var expenses = await Client.GetFromJsonAsync<List<Expense>>(
                "api/expenses");

            if (expenses == null)
            {
                Console.WriteLine("The API returned an empty response.");
                return;
            }

            var pendingClaims = expenses
                .Where(e => string.Equals(
                    e.Status, "Pending",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (pendingClaims.Count == 0)
            {
                Console.WriteLine("No pending expense claims found.");
                return;
            }

            foreach (var expense in pendingClaims)
            {
                DisplayExpense(expense);
                Console.WriteLine("----------------------------------------");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Unable to retrieve claims: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The API request timed out.");
        }
        catch (JsonException)
        {
            Console.WriteLine("The API returned invalid claim data.");
        }
    }

    private static async Task SearchClaimByIdAsync()
    {
        Console.Clear();
        Console.WriteLine("========== SEARCH CLAIM BY ID ==========");
        Console.Write("Enter Claim ID: ");

        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int claimId) || claimId <= 0)
        {
            Console.WriteLine("Invalid Claim ID. Enter a positive whole number.");
            return;
        }

        try
        {
            using var response = await Client.GetAsync(
                $"api/expenses/{claimId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Claim #{claimId} was not found.");
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"The API returned HTTP {(int)response.StatusCode}.");
                return;
            }

            var expense = await response.Content
                .ReadFromJsonAsync<Expense>();

            if (expense == null)
            {
                Console.WriteLine("The API returned no claim details.");
                return;
            }

            DisplayExpense(expense);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Unable to connect to the API: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("The API request timed out.");
        }
        catch (JsonException)
        {
            Console.WriteLine("The API returned invalid claim data.");
        }
    }

    private static void DisplayExpense(Expense expense)
    {
        Console.WriteLine($"Claim ID: {expense.ClaimId}");
        Console.WriteLine($"Employee ID: {expense.EmployeeId}");
        Console.WriteLine($"Employee Name: {expense.EmployeeName}");
        Console.WriteLine($"Category: {expense.Category}");
        Console.WriteLine($"Amount: ₹{expense.Amount:N2}");
        Console.WriteLine($"Expense Date: {expense.ExpenseDate:dd MMM yyyy}");
        Console.WriteLine($"Description: {expense.Description}");
        Console.WriteLine($"Status: {expense.Status}");
        Console.WriteLine($"Created At: {expense.CreatedAt:dd MMM yyyy hh:mm tt}");
    }
}
