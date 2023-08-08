using Basecode.Data.Models;

namespace Basecode.Data.ViewModels;

public class ApplicantDetailsViewModel
{
    public Applicant Applicant { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdateDate { get; set; }
    public List<CharacterReference> CharacterReferences { get; set; }
    public List<BackgroundCheck> BackgroundCheck { get; set; }
}