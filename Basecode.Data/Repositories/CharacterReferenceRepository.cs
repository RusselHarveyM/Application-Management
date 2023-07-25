using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
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
            return this.GetDbSet<CharacterReference>();
        }
    }
}
