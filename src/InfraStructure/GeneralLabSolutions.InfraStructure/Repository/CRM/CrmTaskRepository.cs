using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;

namespace GeneralLabSolutions.InfraStructure.Repository.CRM
{
    public class CrmTaskRepository : GenericRepository<CrmTask, Guid>, ICrmTaskRepository
    {
        public CrmTaskRepository(AppDbContext context) : base(context) { }
    }
}