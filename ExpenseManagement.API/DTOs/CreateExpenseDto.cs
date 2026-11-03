
using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.API.DTOs;

public class CreateExpenseDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Employee ID must be greater than zero.")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Employee name is required.")]
    [StringLength(100,
        ErrorMessage = "Employee name cannot exceed 100 characters.")]
    public string EmployeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [RegularExpression(
        "^(Travel|Food|Accommodation|Office Supplies|Other)$",
        ErrorMessage = "Select a valid expense category.")]
    public string Category { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999.99",
        ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Expense date is required.")]
    [DataType(DataType.Date)]
    public DateTime ExpenseDate { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250,
        ErrorMessage = "Description cannot exceed 250 characters.")]
    public string Description { get; set; } = string.Empty;
}
