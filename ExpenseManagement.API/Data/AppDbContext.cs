using ExpenseManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Expense>();

        entity.HasKey(e => e.ClaimId);

        entity.Property(e => e.ClaimId).UseIdentityColumn();
        entity.Property(e => e.EmployeeName).HasMaxLength(100).IsRequired();
        entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Amount).HasPrecision(12, 2);
        entity.Property(e => e.Description).HasMaxLength(250).IsRequired();
        entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
        entity.Property(e => e.ExpenseDate).HasColumnType("date");
        entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
    }
}