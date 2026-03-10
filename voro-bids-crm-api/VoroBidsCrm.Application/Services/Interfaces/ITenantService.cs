using VoroBidsCrm.Application.DTOs.Public;
using VoroBidsCrm.Application.DTOs.Tenant;
using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface ITenantService : IServiceBase<Tenant>
    {
        Task<Tenant?> GetByIdAsync(Guid id);
        Task<PublicTenantDto?> GetBySlugAsync(string slug);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> CreateAsync(CreateTenantDto dto);
        Task<Tenant> UpdateAsync(Guid id, UpdateTenantDto dto);
        Task<Tenant> UpdateLogoAsync(Guid id, string logoUrl);
        Task<bool> DeleteAsync(Guid id);
    }
}
