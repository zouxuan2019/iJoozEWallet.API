using iJoozEWallet.API.Persistence.Contexts;

namespace iJoozEWallet.API.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext _context;

        internal BaseRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}