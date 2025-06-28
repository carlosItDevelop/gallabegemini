using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;

namespace GeneralLabSolutions.InfraStructure.Repository.CRM
{
    public class ActivityRepository : GenericRepository<Activity, Guid>, IActivityRepository
    {
        public ActivityRepository(AppDbContext context) : base(context) { }
    }
}