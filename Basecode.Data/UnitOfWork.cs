using Basecode.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Basecode.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    public UnitOfWork(BasecodeContext serviceContext)
    {
        Database = serviceContext;
    }

    public void Dispose()
    {
        Database.Dispose();
    }

    public DbContext Database { get; }

    public void SaveChanges()
    {
        Database.SaveChanges();
    }
}