using System.Collections.Generic;
using System.Threading.Tasks;
using iJoozEWallet.API.Domain.Models;
using iJoozEWallet.API.Domain.Repositories;
using iJoozEWallet.API.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace iJoozEWallet.API.Persistence.Repositories
{
    public class EWalletRepository : BaseRepository, IEWalletRepository
    {
        public EWalletRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EWallet>> ListAsync()
        {
            return await _context.EWallet.ToListAsync();
        }

        public Task AddTopUpAsync(TopUpHistory topUpHistory)
        {
            throw new System.NotImplementedException();
        }
    }
}