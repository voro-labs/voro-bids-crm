using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories.Base;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepositoryBase<Employee>
    {
        Task<IEnumerable<Employee>> GetByTenantWithSpecialtiesAsync(Guid tenantId);
        Task<Employee?> GetByIdWithSpecialtiesAsync(Guid id);
        Task UpdateSpecialtiesAsync(Guid employeeId, IEnumerable<Guid> serviceIds);
        Task<IEnumerable<Employee>> GetAvailableForServiceAsync(Guid tenantId, Guid serviceId);
        Task<IEnumerable<Employee>> GetPublicEmployeesByServiceAsync(Guid tenantId, Guid serviceId);
    }
}
