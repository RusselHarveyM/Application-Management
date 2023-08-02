using System.ComponentModel.DataAnnotations;
using Basecode.Data.Models;

namespace Basecode.Data.ViewModels;

public class ApplicantViewModel
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    public string Firstname { get; set; }

    public string? Middlename { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Age is required.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Birthdate is required.")]
    public DateTime Birthdate { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Nationality is required.")]
    public string Nationality { get; set; }

    [Required(ErrorMessage = "Street is required.")]
    public string Street { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Province is required.")]
    public string Province { get; set; }

    [Required(ErrorMessage = "Zip is required.")]
    public string Zip { get; set; }

    [Required(ErrorMessage = "Phone is required.")]
    [StringLength(11, ErrorMessage = "Phone must be 11 numbers long.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "CV is required.")]
    public byte[] CV { get; set; }

    [Required(ErrorMessage = "At least one character reference is required.")]
    public List<CharacterReferenceViewModel> CharacterReferences { get; set; }

    public Application Application { get; set; }

    public int JobOpeningId { get; set; }
}