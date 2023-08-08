using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.Models;

public class Responsibility
{
    public int Id { get; set; }

    public int JobOpeningId { get; set; }

    [Required(ErrorMessage = "The Description is required.")]
    public string Description { get; set; }
}