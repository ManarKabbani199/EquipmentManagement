namespace EquipmentManagement.Domain.Entities;

public class BorrowingRecord
{
    public int Id { get; set; }

    // العلاقة مع المعدّة
    public int EquipmentId { get; set; }

    public Equipment Equipment { get; set; } = null!;

    // العلاقة مع الموظف
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public DateTime BorrowDate { get; set; }

    public DateTime ExpectedReturnDate { get; set; }

    public DateTime? ActualReturnDate { get; set; }

    public bool IsReturned { get; set; } = false;
}