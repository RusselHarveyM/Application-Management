using Basecode.Data.Models;

namespace Basecode.Data.Interfaces
{
    /// <summary>
    /// Represents an interface for the Character Reference repository.
    /// </summary>
    public interface ICharacterReferenceRepository
    {
        /// <summary>
        /// Creates a new character reference.
        /// </summary>
        /// <param name="characterReference">The CharacterReference object to create.</param>
        void CreateReference(CharacterReference characterReference);

        /// <summary>
        /// Gets all character references.
        /// </summary>
        /// <returns>An IQueryable of CharacterReference Object.</returns>
        IQueryable<CharacterReference> GetAll();
    }
}
