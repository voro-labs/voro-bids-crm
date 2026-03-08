using VoroBidsCrm.Domain.Entities.Identity;
using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories.Identity
{
    public class UserRoleRepository(JasmimDbContext context, IUnitOfWork unitOfWork) : RepositoryBase<UserRole>(context, unitOfWork), IUserRoleRepository
    {

    }
}
