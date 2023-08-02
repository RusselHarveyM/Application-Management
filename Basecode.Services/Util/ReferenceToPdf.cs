using Basecode.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Basecode.Services.Util;

public class ReferenceToPdf
{
    private readonly ICharacterReferenceService _characterReferenceService;
    
    public ReferenceToPdf(ICharacterReferenceService characterReferenceService)
    {
        _characterReferenceService = characterReferenceService;
    }
    
    public void ExportToPdf(int id, string filePath)
    {
        // Get the data from the model
        var characterReference = _characterReferenceService.GetCharacterReferenceById(id);

        // Create a new PDF document
        var document = new Document();
        var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        // Add the character reference data to the PDF
        document.Add(new Paragraph($"Name: {characterReference.Name}"));
        document.Add(new Paragraph($"Address: {characterReference.Address}"));
        document.Add(new Paragraph($"Email: {characterReference.Email}"));
        document.Add(new Paragraph($"Applicant: {characterReference.Applicant.Firstname + " " + characterReference.Applicant.Middlename + " " + characterReference.Applicant.Lastname}"));
        if (characterReference.BackgroundCheck != null)
        {
            var backgroundCheck = characterReference.BackgroundCheck;
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
        }


        // Close the PDF document
        document.Close();
    }

    
}