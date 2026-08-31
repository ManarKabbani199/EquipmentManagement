using System.ComponentModel.DataAnnotations;

namespace EquipmentManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "FullNameRequired")]
    [Display(Name = "FullName")]
    public string FullName { get; set; } =
        string.Empty;

    [Required(ErrorMessage = "EmailRequired")]
    [EmailAddress(ErrorMessage = "InvalidEmail")]
    [Display(Name = "Email")]
    public string Email { get; set; } =
        string.Empty;

    [Required(ErrorMessage = "DepartmentRequired")]
    [Display(Name = "Department")]
    public string Department { get; set; } =
        string.Empty;

    public string? IdentityUserId { get; set; }

    public ICollection<BorrowingRecord>
        BorrowingRecords
    {
        get;
        set;
    } = new List<BorrowingRecord>();
}