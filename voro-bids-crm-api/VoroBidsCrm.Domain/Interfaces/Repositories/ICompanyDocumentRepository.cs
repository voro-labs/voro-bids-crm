using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Domain.Interfaces.Repositories
{
    public interface ICompanyDocumentRepository
    {
        Task<IEnumerable<CompanyDocument>> GetAllAsync(Guid tenantId, CancellationToken ct = default);
        Task<CompanyDocument?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);
        Task<CompanyDocument> CreateAsync(CompanyDocument document, CancellationToken ct = default);
        Task<bool> DeleteAsync(CompanyDocument document, CancellationToken ct = default);
    }
}
