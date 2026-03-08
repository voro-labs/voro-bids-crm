using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class TenantRepository(JasmimDbContext context, IUnitOfWork unitOfWork)
        : RepositoryBase<Tenant>(context, unitOfWork), ITenantRepository
    {
        private readonly JasmimDbContext _context = context;

        public async Task<Tenant?> GetBySlugAsync(string slug)
        {
            return await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug && !t.IsDeleted);
        }
    }
}
