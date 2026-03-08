using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class TenantModuleRepository(JasmimDbContext context, IUnitOfWork unitOfWork)
        : RepositoryBase<TenantModule>(context, unitOfWork), ITenantModuleRepository
    {
        private readonly JasmimDbContext _context = context;

        public async Task<IEnumerable<TenantModule>> GetModulesByTenantIdAsync(Guid tenantId)
        {
            return await _context.TenantModules
                .Where(tm => tm.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<TenantModule?> GetModuleAsync(Guid tenantId, AppModule module)
        {
            return await _context.TenantModules
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Module == module);
        }
    }
}
