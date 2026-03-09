using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class AuctionChecklistRepository(JasmimDbContext context, IUnitOfWork unitOfWork) : RepositoryBase<AuctionChecklist>(context, unitOfWork), IAuctionChecklistRepository
    {
    }
}
