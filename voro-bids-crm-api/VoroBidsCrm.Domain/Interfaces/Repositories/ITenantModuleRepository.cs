using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface ITenantModuleRepository : IRepositoryBase<TenantModule>
    {
        Task<IEnumerable<TenantModule>> GetModulesByTenantIdAsync(Guid tenantId);
        Task<TenantModule?> GetModuleAsync(Guid tenantId, AppModule module);
    }
}
