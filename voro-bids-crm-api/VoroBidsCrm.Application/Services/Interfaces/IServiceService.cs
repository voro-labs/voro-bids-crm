using VoroBidsCrm.Application.DTOs.CRM;
using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IServiceService
    {
        Task<ServiceDto> CreateAsync(CreateServiceDto dto);
        Task<ServiceDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ServiceDto>> GetAllAsync();
        Task<ServiceDto> UpdateAsync(Guid id, UpdateServiceDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
