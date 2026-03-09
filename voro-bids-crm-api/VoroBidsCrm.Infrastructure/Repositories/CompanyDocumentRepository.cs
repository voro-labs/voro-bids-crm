using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class CompanyDocumentRepository : ICompanyDocumentRepository
    {
        private readonly DbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyDocumentRepository(DbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CompanyDocument>> GetAllAsync(Guid tenantId, CancellationToken ct = default)
        {
            return await _context.Set<CompanyDocument>()
                .Where(d => d.TenantId == tenantId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<CompanyDocument?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default)
        {
            return await _context.Set<CompanyDocument>()
                .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId && !d.IsDeleted, ct);
        }

        public async Task<CompanyDocument> CreateAsync(CompanyDocument document, CancellationToken ct = default)
        {
            _context.Set<CompanyDocument>().Add(document);
            await _unitOfWork.CommitAsync(ct);
            return document;
        }

        public async Task<bool> DeleteAsync(CompanyDocument document, CancellationToken ct = default)
        {
            // Soft delete
            document.IsDeleted = true;
            document.DeletedAt = DateTimeOffset.UtcNow;

            _context.Set<CompanyDocument>().Update(document);
            await _unitOfWork.CommitAsync(ct);
            return true;
        }
    }
}
