using System.Linq.Expressions;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface IAuctionRepository : IRepositoryBase<Auction>
    {
         Task<int> CountAsync(Expression<Func<Auction, bool>> predicate);
    }
}
