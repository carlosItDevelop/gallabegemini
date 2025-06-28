using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;

namespace GeneralLabSolutions.InfraStructure.Repository.CRM
{
    public class LeadRepository : GenericRepository<Lead, Guid>, ILeadRepository
    {
        public LeadRepository(AppDbContext context) : base(context) { }

        // Exemplo de como um método futuro seria implementado:
        // public async Task<Lead> ObterLeadComTarefas(Guid id)
        // {
        //     return await _context.Leads
        //         .Include(l => l.Tasks)
        //         .FirstOrDefaultAsync(l => l.Id == id);
        // }
    }
}