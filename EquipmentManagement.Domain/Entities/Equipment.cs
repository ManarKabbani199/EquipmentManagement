namespace EquipmentManagement.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public bool IsAvailable { get; set; } = true;

    // المفتاح الأجنبي للتصنيف
    public int CategoryId { get; set; }

    // التصنيف المرتبط بالمعدّة
    public Category? Category { get; set; }

    public string? ImagePath { get; set; }

    // عمليات استعارة هذه المعدّة
    public ICollection<BorrowingRecord> BorrowingRecords { get; set; }
        = new List<BorrowingRecord>();
}