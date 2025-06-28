using GeneralLabSolutions.Domain.Entities.CRM;

namespace GeneralLabSolutions.Domain.Interfaces.CRM
{
    public interface IActivityRepository : IGenericRepository<Activity, Guid>
    {
        // Métodos específicos para atividades/agenda
    }
}