using System.ComponentModel.DataAnnotations;

namespace EquipmentManagement.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "CategoryNameRequired")]
    [Display(Name = "Name")]
    public string Name { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage = "CategoryDescriptionRequired"
    )]
    [Display(Name = "Description")]
    public string Description { get; set; } =
        string.Empty;

    public ICollection<Equipment> Equipments
    {
        get;
        set;
    } = new List<Equipment>();
}