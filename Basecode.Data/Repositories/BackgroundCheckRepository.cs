using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Basecode.Data.Repositories;

public class BackgroundCheckRepository : BaseRepository, IBackgroundCheckRepository
{
    private readonly BasecodeContext _context;

    public BackgroundCheckRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public int Create(BackgroundCheck form)
    {
        _context.BackgroundCheck.Add(form);
        _context.SaveChanges();

        return form.Id;
    }

    public IQueryable<BackgroundCheck> GetAll()
    {
        return GetDbSet<BackgroundCheck>();
    }

    public BackgroundCheck GetById(int id)
    {
        return _context.BackgroundCheck.Include(bc => bc.CharacterReference).FirstOrDefault(bc => bc.Id == id);
    }
}