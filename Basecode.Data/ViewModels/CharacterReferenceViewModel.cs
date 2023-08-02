using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels;

public class CharacterReferenceViewModel
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    public int ApplicantId { get; set; }
}