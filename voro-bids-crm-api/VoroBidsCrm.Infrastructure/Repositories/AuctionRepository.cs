using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class AuctionRepository(JasmimDbContext context, IUnitOfWork unitOfWork)
        : RepositoryBase<Auction>(context, unitOfWork), IAuctionRepository
    {

        public async Task<int> CountAsync(Expression<Func<Auction, bool>> predicate)
        {
            return await _dbSet.Where(predicate).CountAsync();
        }
    }
}
