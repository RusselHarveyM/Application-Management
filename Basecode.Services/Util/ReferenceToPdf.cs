using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Basecode.Services.Util;

public class ReferenceToPdf
{
    private readonly ICharacterReferenceService _characterReferenceService;
    private readonly IBackgroundCheckService _backgroundCheckService;

    public ReferenceToPdf(ICharacterReferenceService characterReferenceService,
        IBackgroundCheckService backgroundCheckService)
    {
        _characterReferenceService = characterReferenceService;
        _backgroundCheckService = backgroundCheckService;
    }

    public void ExportToPdf(BackgroundCheck backgroundCheck, string filePath)
    {
        // Create a new PDF document
        var document = new Document();
        var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        document.Add(new Paragraph("Background Check:"));
        document.Add(new Paragraph($"Firstname: {backgroundCheck.Firstname}"));
        document.Add(new Paragraph($"Lastname: {backgroundCheck.Lastname}"));
        document.Add(new Paragraph($"Email: {backgroundCheck.Email}"));
        document.Add(new Paragraph($"PhoneNumber: {backgroundCheck.PhoneNumber}"));
        document.Add(new Paragraph($"Relationship: {backgroundCheck.Relationship}"));
        document.Add(new Paragraph($"AnsweredDate: {backgroundCheck.AnsweredDate.ToShortDateString()}"));
        if (!string.IsNullOrEmpty(backgroundCheck.Q1))
        {
            document.Add(new Paragraph($"Q1: {backgroundCheck.Q1}"));
        }

        if (!string.IsNullOrEmpty(backgroundCheck.Q2))
        {
            document.Add(new Paragraph($"Q2: {backgroundCheck.Q2}"));
        }

        if (!string.IsNullOrEmpty(backgroundCheck.Q3))
        {
            document.Add(new Paragraph($"Q3: {backgroundCheck.Q3}"));
        }

        if (!string.IsNullOrEmpty(backgroundCheck.Q4))
        {
            document.Add(new Paragraph($"Q4: {backgroundCheck.Q4}"));
        }

        // Close the PDF document
        document.Close();
    }
}