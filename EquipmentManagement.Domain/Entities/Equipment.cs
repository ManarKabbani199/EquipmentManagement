using System.ComponentModel.DataAnnotations;

namespace EquipmentManagement.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "EquipmentNameRequired")]
    [Display(Name = "EquipmentName")]
    public string Name { get; set; } =
        string.Empty;

    [Required(ErrorMessage = "SerialNumberRequired")]
    [Display(Name = "SerialNumber")]
    public string SerialNumber { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage = "EquipmentDescriptionRequired"
    )]
    [Display(Name = "Description")]
    public string Description { get; set; } =
        string.Empty;

    [Required(ErrorMessage = "PurchaseDateRequired")]
    [DataType(DataType.Date)]
    [Display(Name = "PurchaseDate")]
    public DateTime PurchaseDate { get; set; }

    [Required(ErrorMessage = "PurchasePriceRequired")]
    [Range(
        0,
        double.MaxValue,
        ErrorMessage = "PurchasePriceNonNegative"
    )]
    [Display(Name = "PurchasePrice")]
    public decimal PurchasePrice { get; set; }


    [Display(Name = "AvailableForBorrowing")]
    public bool IsAvailable { get; set; } = true;

    [Required(ErrorMessage = "CategoryRequired")]
    [Range(
    1,
    int.MaxValue,
    ErrorMessage = "CategoryRequired"
)]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }


    public Category? Category { get; set; }

    public string? ImagePath { get; set; }

    public ICollection<BorrowingRecord>
        BorrowingRecords
    {
        get;
        set;
    } = new List<BorrowingRecord>();
}