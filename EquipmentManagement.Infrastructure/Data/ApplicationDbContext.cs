using EquipmentManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace EquipmentManagement.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Equipment> Equipment { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<BorrowingRecord> BorrowingRecords { get; set; }

    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipment>()
            .Property(e => e.PurchasePrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Equipment>()
        .HasIndex(e => e.SerialNumber)
        .IsUnique()
        .HasDatabaseName(
            "IX_Equipment_SerialNumber"
        );

        modelBuilder.Entity<Employee>()
        .HasIndex(e => e.Email)
        .IsUnique()
        .HasDatabaseName(
            "IX_Employees_Email"
        );

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.IdentityUserId)
            .IsUnique()
            .HasDatabaseName(
                "IX_Employees_IdentityUserId"
            );

        modelBuilder.Entity<BorrowingRecord>()
            .HasIndex(b => b.BorrowDate)
            .HasDatabaseName(
                "IX_BorrowingRecords_BorrowDate"
            );

        modelBuilder.Entity<BorrowingRecord>()
            .HasIndex(b => new
            {
                b.IsReturned,
                b.ExpectedReturnDate
            })
            .HasDatabaseName(
                "IX_BorrowingRecords_IsReturned_ExpectedReturnDate"
            );

        modelBuilder.Entity<BorrowingRecord>()
            .HasIndex(b => new
            {
                b.EmployeeId,
                b.BorrowDate
            })
            .HasDatabaseName(
                "IX_BorrowingRecords_EmployeeId_BorrowDate"
            );
    }
}