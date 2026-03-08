using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using VoroBidsCrm.Domain.Entities.Identity;
using VoroBidsCrm.Infrastructure.Repositories.Base;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;

namespace VoroBidsCrm.Infrastructure.Repositories.Identity
{
    public class RoleRepository(JasmimDbContext context, IUnitOfWork unitOfWork) : RepositoryBase<Role>(context, unitOfWork), IRoleRepository
    {
       
    }
}
