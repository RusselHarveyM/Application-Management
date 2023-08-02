using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories;

public class QualificationRepository : BaseRepository, IQualificationRepository
{
    private readonly BasecodeContext _context;

    public QualificationRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public IQueryable<Qualification> GetAll()
    {
        return GetDbSet<Qualification>();
    }

    public void AddQualification(Qualification qualification)
    {
        _context.Qualification.Add(qualification);
        _context.SaveChanges();
    }

    public Qualification GetQualificationById(int id)
    {
        return _context.Qualification.Find(id);
    }

    public void UpdateQualification(Qualification qualification)
    {
        _context.Qualification.Update(qualification);
        _context.SaveChanges();
    }

    public void DeleteQualification(Qualification qualification)
    {
        _context.Qualification.Remove(qualification);
        _context.SaveChanges();
    }
}