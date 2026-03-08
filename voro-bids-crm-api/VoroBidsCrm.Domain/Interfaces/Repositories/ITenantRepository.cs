using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface ITenantRepository : IRepositoryBase<Tenant>
    {
        Task<Tenant?> GetBySlugAsync(string slug);
    }
}
