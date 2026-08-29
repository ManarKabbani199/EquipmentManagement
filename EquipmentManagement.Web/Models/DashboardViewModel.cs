using EquipmentManagement.Domain.Entities;

namespace EquipmentManagement.Web.Models;

public class DashboardViewModel
{
    public int TotalEquipment { get; set; }

    public int AvailableEquipment { get; set; }

    public int BorrowedEquipment { get; set; }

    public int TotalEmployees { get; set; }

    public int ActiveBorrowings { get; set; }

    public int OverdueBorrowings { get; set; }

    public List<BorrowingRecord> RecentBorrowings { get; set; }
        = new List<BorrowingRecord>();
}