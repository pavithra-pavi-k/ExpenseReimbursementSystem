using ExpenseManagement.API.DTOs;
using ExpenseManagement.API.Models;
using ExpenseManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagement.API.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _service;

    public ExpensesController(ExpenseService service)
    {
        _service = service;
    }

    // GET: /api/expenses
    [HttpGet]
    public async Task<ActionResult<List<Expense>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET: /api/expenses/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Expense>> GetById(int id)
    {
        var expense = await _service.GetByIdAsync(id);

        if (expense is null)
            return NotFound(new { message = "Expense claim not found." });

        return Ok(expense);
    }

    // GET: /api/expenses/employee/101
    [HttpGet("employee/{employeeId:int}")]
    public async Task<ActionResult<List<Expense>>> GetByEmployeeId(int employeeId)
    {
        if (employeeId <= 0)
            return BadRequest(new { message = "Employee ID must be positive." });

        return Ok(await _service.GetByEmployeeIdAsync(employeeId));
    }

    // POST: /api/expenses
    [HttpPost]
    public async Task<ActionResult<Expense>> Create(CreateExpenseDto dto)
    {
        try
        {
            var expense = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = expense.ClaimId },
                expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: /api/expenses/5/status
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<Expense>> UpdateStatus(
        int id,
        UpdateStatusDto dto)
    {
        try
        {
            var expense = await _service.UpdateStatusAsync(id, dto.Status);
            return Ok(expense);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: /api/expenses/summary
    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryDto>> GetSummary()
    {
        return Ok(await _service.GetSummaryAsync());
    }
}