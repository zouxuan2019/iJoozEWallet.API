using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<EWallet>> ListAllAsync()
        {
            return await _context.EWallet
                .Include(e=>e.TopUpHistories)
                .Include(e=>e.DeductHistories)
                .ToListAsync();
        }

        public void AddOrUpdateEWallet(EWallet eWallet, bool isAdd)
        {
            if (isAdd)
            {
                _context.EWallet.Add(eWallet);
            }
            else
            {
                _context.EWallet.Update(eWallet);
            }
        }

        public async Task<EWallet> FindByUserIdAsync(int userId)
        {
            return await _context.EWallet
                .Include(e=>e.TopUpHistories)
                .Include(e=>e.DeductHistories)
                .SingleOrDefaultAsync(e=>e.UserId==userId);
        }

        public async Task<TopUpHistory> FindByTopUpTransactionIdAsync(string transactionId)
        {
            return await _context.TopUpHistories.SingleOrDefaultAsync(t => t.TransactionId == transactionId);
        }
    }
}