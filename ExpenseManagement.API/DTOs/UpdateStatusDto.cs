using System.ComponentModel.DataAnnotations;

namespace ExpenseManagement.API.DTOs;

public class UpdateStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}