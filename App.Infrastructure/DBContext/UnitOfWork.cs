using App.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.DBContext;

public sealed class UnitOfWork<TContext>(TContext context, ILogger<UnitOfWork<TContext>> logger) : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context = context;
    private readonly ILogger<UnitOfWork<TContext>> _logger = logger;

    public async Task<bool> CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.LogError(dbUpdateException, "An error occured during commiting changes");
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }
}