using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.API.Models;

public class Expense
{
    public int ClaimId { get; set; }

    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Required, StringLength(100)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    [Required, StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }
}