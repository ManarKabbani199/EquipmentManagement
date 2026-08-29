namespace EquipmentManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string? IdentityUserId { get; set; }

    public ICollection<BorrowingRecord> BorrowingRecords { get; set; }
        = new List<BorrowingRecord>();
}