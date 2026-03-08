using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using VoroBidsCrm.Domain.Entities.Identity;
using VoroBidsCrm.Application.Services.Base;
using VoroBidsCrm.Application.Services.Interfaces.Identity;

namespace VoroBidsCrm.Application.Services.Identity
{
    public class UserRoleService(IUserRoleRepository userRoleRepository) : ServiceBase<UserRole>(userRoleRepository), IUserRoleService
    {
        
    }
}
