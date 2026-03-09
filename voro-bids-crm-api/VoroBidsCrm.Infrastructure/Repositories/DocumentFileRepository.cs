using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Repositories.Base;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class DocumentFileRepository : RepositoryBase<DocumentFile>, IDocumentFileRepository
    {
        public DocumentFileRepository(DbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }
    }
}
