using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class ServiceRepository(JasmimDbContext context, IUnitOfWork unitOfWork)
        : RepositoryBase<Service>(context, unitOfWork), IServiceRepository
    {
        private readonly JasmimDbContext _context = context;

        public async Task<IEnumerable<Service>> GetPublicActiveByTenantAsync(Guid tenantId)
        {
            return await _context.Services
                .IgnoreQueryFilters()
                .Where(s => s.TenantId == tenantId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<Service?> GetPublicByIdAsync(Guid tenantId, Guid serviceId)
        {
            return await _context.Services
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.TenantId == tenantId);
        }
    }
}
