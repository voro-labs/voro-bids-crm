using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Infrastructure.Repositories.Base;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Factories;

namespace VoroBidsCrm.Infrastructure.Repositories
{
    public class NotificationRepository(JasmimDbContext context, IUnitOfWork unitOfWork) : RepositoryBase<Notification>(context, unitOfWork), INotificationRepository
    {

    }
}
