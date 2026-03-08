using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface IServiceRepository : IRepositoryBase<Service>
    {
        Task<IEnumerable<Service>> GetPublicActiveByTenantAsync(Guid tenantId);
        Task<Service?> GetPublicByIdAsync(Guid tenantId, Guid serviceId);
    }
}
