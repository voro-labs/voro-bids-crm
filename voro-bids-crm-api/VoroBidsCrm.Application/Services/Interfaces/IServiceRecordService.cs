using VoroBidsCrm.Application.DTOs.CRM;
using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Enums;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IServiceRecordService
    {
        Task<ServiceRecordDto> CreateAsync(CreateServiceRecordDto dto);
        Task<ServiceRecordDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ServiceRecordDto>> GetAllAsync();
        Task<IEnumerable<ServiceRecordDto>> GetByClientIdAsync(Guid clientId);
        Task<ServiceRecordDto> UpdateAsync(Guid id, UpdateServiceRecordDto dto);
        Task<bool> UpdateStatusAsync(Guid id, AppointmentStatus status);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteByAppointmentIdAsync(Guid appointmentId);
    }
}
