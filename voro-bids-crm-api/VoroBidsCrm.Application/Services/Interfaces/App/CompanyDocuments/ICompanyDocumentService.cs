using Microsoft.AspNetCore.Http;
using VoroBidsCrm.Application.DTOs.CompanyDocuments;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.Interfaces.App.CompanyDocuments
{
    public interface ICompanyDocumentService
    {
        Task<ResponseViewModel<List<CompanyDocumentDto>>> GetAllAsync(Guid tenantId, CancellationToken ct = default);
        Task<ResponseViewModel<CompanyDocumentDto>> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);
        Task<ResponseViewModel<CompanyDocumentDto>> CreateAsync(Guid tenantId, CreateCompanyDocumentDto dto, CancellationToken ct = default);
        Task<ResponseViewModel<CompanyDocumentDto>> UpdateAsync(Guid id, Guid tenantId, UpdateCompanyDocumentDto dto, CancellationToken ct = default);
        Task<ResponseViewModel<object?>> DeleteAsync(Guid id, Guid tenantId, CancellationToken ct = default);
    }
}
