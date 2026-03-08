using Microsoft.AspNetCore.Http;
using VoroBidsCrm.Application.DTOs.Employee;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(Guid id);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
        Task UpdateAsync(Guid id, UpdateEmployeeDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<EmployeeDto>> GetAvailableForServiceAsync(Guid serviceId);
        Task<string> UploadPhotoAsync(Guid id, IFormFile file);
    }
}
