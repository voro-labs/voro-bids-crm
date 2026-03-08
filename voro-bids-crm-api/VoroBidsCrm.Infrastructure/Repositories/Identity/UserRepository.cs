using VoroBidsCrm.Domain.Entities.Identity;
using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories.Identity
{
    public class UserRepository(JasmimDbContext context, IUnitOfWork unitOfWork) : RepositoryBase<User>(context, unitOfWork), IUserRepository
    {
    }
}
