using VoroBidsCrm.Application.DTOs.Tenant;
using VoroBidsCrm.Domain.Enums;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface ITenantModuleService
    {
        Task<IEnumerable<TenantModuleDto>> GetMyModulesAsync();
        Task UpdateModuleAsync(AppModule module, UpdateTenantModuleDto dto);
        Task<bool> IsModuleEnabledAsync(Guid tenantId, AppModule module);
    }
}
