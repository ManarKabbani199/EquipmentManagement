using System.ComponentModel.DataAnnotations;

namespace EquipmentManagement.Web.Models;

public class CreateEmployeeViewModel
{
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

    [Required(ErrorMessage = "PasswordRequired")]
    [DataType(DataType.Password)]
    [MinLength(
        6,
        ErrorMessage = "PasswordMinimumLength"
    )]
    [Display(Name = "Password")]
    public string Password { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage = "ConfirmPasswordRequired"
    )]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "PasswordsDoNotMatch"
    )]
    [Display(Name = "ConfirmPassword")]
    public string ConfirmPassword { get; set; } =
        string.Empty;
}