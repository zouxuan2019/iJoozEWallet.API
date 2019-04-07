using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Persistence.Contexts;

namespace iJoozEWallet.API.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}