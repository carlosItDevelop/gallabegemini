using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class KanbanTaskRepository : GenericRepository<KanbanTask, Guid>, IKanbanTaskRepository
    {
        // ToDo: Implementar métodos específicos para KanbanTask;

        public KanbanTaskRepository(AppDbContext context) : base(context){}

        //public async Task<IEnumerable<KanbanTask>> BuscaSemNoTrakingComParticipante(Expression<Func<KanbanTask, bool>> predicate)
        //{
        //    if (predicate == null)
        //    {
        //        return await _context.Set<KanbanTask>()
        //            .Include(x => x.Participantes)
        //            .ToListAsync();
        //    }
        //    return await _context.Set<KanbanTask>()
        //        .Include(x => x.Participantes)
        //        .Where(predicate).ToListAsync();
        //}


        //public async Task<IEnumerable<KanbanTask>> GetAllWithParticipanteAsync()
        //{
        //    return await _context.Set<KanbanTask>()
        //        .Include(x => x.Participantes)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}


    }
}
