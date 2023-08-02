using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories;

public class CharacterReferenceRepository : BaseRepository, ICharacterReferenceRepository
{
    private readonly BasecodeContext _context;

    public CharacterReferenceRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public void CreateReference(CharacterReference characterReference)
    {
        _context.CharacterReference.Add(characterReference);
        _context.SaveChanges();
    }

    public IQueryable<CharacterReference> GetAll()
    {
        return GetDbSet<CharacterReference>();
    }

    public CharacterReference? GetCharacterReferenceById(int characterReferenceId)
    {
        return _context.CharacterReference.Find(characterReferenceId);
    }
}