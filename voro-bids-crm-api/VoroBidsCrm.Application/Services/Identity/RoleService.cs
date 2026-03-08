using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities.Identity;
using VoroBidsCrm.Application.Services.Base;
using VoroBidsCrm.Application.Services.Interfaces.Identity;

namespace VoroBidsCrm.Application.Services.Identity
{
    public class RoleService(IRoleRepository roleRepository) : ServiceBase<Role>(roleRepository), IRoleService
    {
        public async Task<Role?> GetByNameAsync(string roleName)
            => await roleRepository.Query(r => r.Name == roleName).FirstOrDefaultAsync();
    }
}
