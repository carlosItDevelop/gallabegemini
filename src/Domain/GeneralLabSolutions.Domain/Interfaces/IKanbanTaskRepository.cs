using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IKanbanTaskRepository : IGenericRepository<KanbanTask, Guid>
    {
        //Task<IEnumerable<KanbanTask>> BuscaSemNoTrakingComParticipante(Expression<Func<KanbanTask, bool>> predicate);
        //Task<IEnumerable<KanbanTask>> GetAllWithParticipanteAsync();
    }
}
