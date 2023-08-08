using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

/// <summary>
///     Represents an interface for the Character Reference service.
/// </summary>
public interface ICharacterReferenceService
{
    /// <summary>
    ///     Creates a new character reference for the specified applicant.
    /// </summary>
    /// s
    /// <param name="characterReference">The CharacterReferenceViewModel object containing the character reference data.</param>
    /// <param name="applicantId">The ID of the associated applicant.</param>
    /// <returns>A LogContent object representing the result of the operation.</returns>
    LogContent Create(CharacterReferenceViewModel characterReference, int applicantId);

    /// <summary>
    ///     Retrieves a list of character references associated with the specified applicant ID.
    /// </summary>
    /// <param name="applicantId">The unique identifier of the applicant whose character references are to be retrieved.</param>
    /// <returns>A list of CharacterReference objects representing the character references related to the specified applicant.</returns>
    List<CharacterReference> GetReferencesByApplicantId(int applicantId);

    /// <summary>
    ///     Gets the character reference by identifier.
    /// </summary>
    /// <param name="characterReferenceId">The character reference identifier.</param>
    /// <returns></returns>
    CharacterReference GetCharacterReferenceById(int characterReferenceId);

    /// <summary>
    ///     Gets the character reference applicant identifier.
    /// </summary>
    /// <param name="characterReferenceId">The character reference identifier.</param>
    /// <returns></returns>
    int GetApplicantIdByCharacterReferenceId(int characterReferenceId);
}