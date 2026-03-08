using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface IClientRepository : IRepositoryBase<Client>
    {
        Task<Client?> GetByPhoneAsync(Guid tenantId, string phone);
    }
}
