using ExpenseManagement.API.Data;
using ExpenseManagement.API.DTOs;
using ExpenseManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.API.Services;

public class ExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _context.Expenses
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses
            .FirstOrDefaultAsync(e => e.ClaimId == id);
    }

    public async Task<List<Expense>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.Expenses
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Expense> CreateAsync(CreateExpenseDto dto)
    {
        var category = dto.Category.Trim();
        var employeeName = dto.EmployeeName.Trim();
        var description = dto.Description.Trim();

        string[] allowedCategories =
        [
            "Travel", "Food", "Accommodation",
            "Office Supplies", "Other"
        ];

        if (dto.EmployeeId <= 0)
            throw new ArgumentException("Employee ID must be positive.");

        if (string.IsNullOrWhiteSpace(employeeName))
            throw new ArgumentException("Employee name is required.");

        if (employeeName.Length > 100)
            throw new ArgumentException("Employee name cannot exceed 100 characters.");

        if (!allowedCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Invalid expense category.");

        if (dto.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (dto.ExpenseDate.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("Expense date cannot be in the future.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        if (description.Length > 250)
            throw new ArgumentException("Description cannot exceed 250 characters.");

        var expense = new Expense
        {
            EmployeeId = dto.EmployeeId,
            EmployeeName = employeeName,
            Category = allowedCategories.First(
                c => c.Equals(category, StringComparison.OrdinalIgnoreCase)),
            Amount = dto.Amount,
            ExpenseDate = dto.ExpenseDate.Date,
            Description = description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return expense;
    }

    public async Task<Expense> UpdateStatusAsync(int id, string newStatus)
    {
        var expense = await GetByIdAsync(id);

        if (expense is null)
            throw new KeyNotFoundException("Expense claim not found.");

        newStatus = newStatus.Trim();

        bool allowed = expense.Status switch
        {
            "Pending" =>
                newStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
                newStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase),

            "Approved" =>
                newStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase),

            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException(
                $"Cannot change status from {expense.Status} to {newStatus}.");

        expense.Status = newStatus.ToLowerInvariant() switch
        {
            "approved" => "Approved",
            "rejected" => "Rejected",
            "paid" => "Paid",
            _ => expense.Status
        };

        await _context.SaveChangesAsync();

        return expense;
    }

    public async Task<ExpenseSummaryDto> GetSummaryAsync()
    {
        var expenses = await _context.Expenses.ToListAsync();

        return new ExpenseSummaryDto
        {
            TotalClaims = expenses.Count,
            PendingClaims = expenses.Count(e => e.Status == "Pending"),
            ApprovedClaims = expenses.Count(e => e.Status == "Approved"),
            RejectedClaims = expenses.Count(e => e.Status == "Rejected"),
            PaidClaims = expenses.Count(e => e.Status == "Paid"),

            ApprovedAmount = expenses
                .Where(e => e.Status == "Approved")
                .Sum(e => e.Amount),

            RejectedAmount = expenses
                .Where(e => e.Status == "Rejected")
                .Sum(e => e.Amount),

            PaidAmount = expenses
                .Where(e => e.Status == "Paid")
                .Sum(e => e.Amount)
        };
    }
}