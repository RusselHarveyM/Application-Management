using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories;

public class ResponsibilityRepository : BaseRepository, IResponsibilityRepository
{
    private readonly BasecodeContext _context;

    public ResponsibilityRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public IQueryable<Responsibility> GetAll()
    {
        return GetDbSet<Responsibility>();
    }

    public void AddResponsibility(Responsibility responsibility)
    {
        _context.Responsibility.Add(responsibility);
        _context.SaveChanges();
    }

    public Responsibility GetResponsibilityById(int id)
    {
        return _context.Responsibility.Find(id);
    }

    public void UpdateResponsibility(Responsibility responsibility)
    {
        _context.Responsibility.Update(responsibility);
        _context.SaveChanges();
    }

    public void DeleteResponsibility(Responsibility responsibility)
    {
        _context.Responsibility.Remove(responsibility);
        _context.SaveChanges();
    }
}