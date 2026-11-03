
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ExpenseManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.Web.Controllers;

public class ExpensesController : Controller
{
    private readonly HttpClient _httpClient;

    public ExpensesController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();

        var apiBaseUrl = configuration["ApiSettings:BaseUrl"];

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            throw new InvalidOperationException(
                "API base URL is missing from appsettings.json.");
        }

        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    // Display all expense claims
    public async Task<IActionResult> Index()
    {
        try
        {
            var expenses = await _httpClient
                .GetFromJsonAsync<List<Expense>>("api/expenses");

            if (expenses == null)
            {
                ViewBag.Error =
                    "Unable to load expense claims from the API.";

                return View(new List<Expense>());
            }

            return View(expenses);
        }
        catch (HttpRequestException)
        {
            ViewBag.Error =
                "Unable to connect to the API. Make sure the API is running.";

            return View(new List<Expense>());
        }
        catch (TaskCanceledException)
        {
            ViewBag.Error =
                "The API request timed out. Please try again.";

            return View(new List<Expense>());
        }
    }

    // Display details of a specific expense claim
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            ViewBag.Error = "Please provide a valid claim ID.";
            return View("Details", (Expense?)null);
        }

        try
        {
            using var response = await _httpClient.GetAsync(
                $"api/expenses/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Error =
                    $"Expense claim #{id} was not found.";

                return View("Details", (Expense?)null);
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Unable to retrieve the expense claim. Please try again.";

                return View("Details", (Expense?)null);
            }

            var expense = await response.Content
                .ReadFromJsonAsync<Expense>();

            if (expense == null)
            {
                ViewBag.Error =
                    $"Expense claim #{id} was not found.";

                return View("Details", (Expense?)null);
            }

            return View(expense);
        }
        catch (HttpRequestException)
        {
            ViewBag.Error =
                "Unable to connect to the API. Make sure the API is running.";

            return View("Details", (Expense?)null);
        }
        catch (TaskCanceledException)
        {
            ViewBag.Error =
                "The API request timed out. Please try again.";

            return View("Details", (Expense?)null);
        }
    }

    // Display the create form
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Expense
        {
            ExpenseDate = DateTime.Today
        });
    }

    // Submit a new expense claim
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (!ModelState.IsValid)
        {
            return View(expense);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/expenses",
                new
                {
                    expense.EmployeeId,
                    expense.EmployeeName,
                    expense.Category,
                    expense.Amount,
                    expense.ExpenseDate,
                    expense.Description
                });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Expense claim submitted successfully.";

                return RedirectToAction(nameof(Index));
            }

            var errorBody =
                await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                string.Empty,
                $"Unable to submit claim: {errorBody}");

            return View(expense);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to the API. Please try again.");

            return View(expense);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                string.Empty,
                "The API request timed out. Please try again.");

            return View(expense);
        }
    }

    // Approve, reject, or mark a claim as paid
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id,
        string status)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/expenses/{id}/status",
                new { Status = status });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    $"Claim #{id} status updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            var errorBody =
                await response.Content.ReadAsStringAsync();

            TempData["Error"] =
                $"Unable to update claim: {errorBody}";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Error"] =
                "Unable to connect to the API. Please try again.";

            return RedirectToAction(nameof(Index));
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] =
                "The API request timed out. Please try again.";

            return RedirectToAction(nameof(Index));
        }
    }

    // Display the expense summary dashboard and latest claims
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.LatestExpenses = new List<Expense>();

        try
        {
            var summary = await _httpClient
                .GetFromJsonAsync<ExpenseSummary>(
                    "api/expenses/summary");

            var expenses = await _httpClient
                .GetFromJsonAsync<List<Expense>>(
                    "api/expenses");

            if (summary == null || expenses == null)
            {
                ViewBag.Error =
                    "Unable to load dashboard data from the API.";

                return View(
                    "Dashboard",
                    summary ?? new ExpenseSummary());
            }

            ViewBag.LatestExpenses = expenses
                .OrderByDescending(e => e.ClaimId)
                .Take(5)
                .ToList();

            return View("Dashboard", summary);
        }
        catch (HttpRequestException)
        {
            ViewBag.Error =
                "Unable to connect to the API. Make sure the API is running.";

            return View(
                "Dashboard",
                new ExpenseSummary());
        }
        catch (TaskCanceledException)
        {
            ViewBag.Error =
                "The API request timed out. Please try again.";

            return View(
                "Dashboard",
                new ExpenseSummary());
        }
    }
}
