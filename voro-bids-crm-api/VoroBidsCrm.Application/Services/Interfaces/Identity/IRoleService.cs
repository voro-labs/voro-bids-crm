using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities.Identity;

namespace VoroBidsCrm.Application.Services.Interfaces.Identity
{
    public interface IRoleService : IServiceBase<Role>
    {
        Task<Role?> GetByNameAsync(string roleName);
    }
}
