using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class ClientRepository(JasmimDbContext context, IUnitOfWork unitOfWork)
        : RepositoryBase<Client>(context, unitOfWork), IClientRepository
    {
        private readonly JasmimDbContext _context = context;

        public async Task<Client?> GetByPhoneAsync(Guid tenantId, string phone)
        {
            return await _context.Clients
                .AsQueryable()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Phone == phone && !c.IsDeleted);
        }
    }
}
