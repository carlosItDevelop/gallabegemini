using GeneralLabSolutions.Domain.Entities.CRM;

namespace GeneralLabSolutions.Domain.Interfaces.CRM
{
    public interface ILeadRepository : IGenericRepository<Lead, Guid>
    {
        // Aqui podemos adicionar assinaturas de métodos específicos para Leads no futuro,
        // como por exemplo: Task<Lead> ObterLeadComTarefas(Guid id);
    }
}