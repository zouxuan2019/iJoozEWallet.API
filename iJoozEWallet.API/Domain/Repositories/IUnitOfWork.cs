using System.Threading.Tasks;

namespace iJoozEWallet.API.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}