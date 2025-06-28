using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;

namespace GeneralLabSolutions.Domain.Interfaces.CRM
{
    public interface ICrmTaskRepository : IGenericRepository<CrmTask, Guid>
    {
        // Métodos específicos para tarefas do CRM
    }
}