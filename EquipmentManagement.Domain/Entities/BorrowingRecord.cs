namespace EquipmentManagement.Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class BorrowingRecord
{
    public int Id { get; set; }


    [Required(ErrorMessage = "EquipmentRequired")]
    [Range(
    1,
    int.MaxValue,
    ErrorMessage = "EquipmentRequired"
)]
    [Display(Name = "Equipment")]
    public int EquipmentId { get; set; }

    public Equipment Equipment { get; set; } =
        null!;

    [Required(ErrorMessage = "EmployeeRequired")]
    [Range(
     1,
     int.MaxValue,
     ErrorMessage = "EmployeeRequired"
 )]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } =
        null!;

    [Display(Name = "BorrowDate")]
    public DateTime BorrowDate { get; set; }

    [Display(Name = "ExpectedReturnDate")]
    public DateTime ExpectedReturnDate { get; set; }

    [Display(Name = "ActualReturnDate")]
    public DateTime? ActualReturnDate { get; set; }

    [Display(Name = "Returned")]
    public bool IsReturned { get; set; } = false;
}