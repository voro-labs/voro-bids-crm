using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IUserExtensionService : IServiceBase<UserExtension>
    {
        Task<IEnumerable<UserExtensionDto>> GetAllAsync();
        Task<UserExtensionDto?> GetByIdAsync(Guid id);
        Task<UserExtensionDto> CreateAsync(UserExtensionDto model);
        Task<UserExtensionDto> UpdateAsync(Guid id, UserExtensionDto model);
        Task DeleteAsync(Guid id);
    }
}
