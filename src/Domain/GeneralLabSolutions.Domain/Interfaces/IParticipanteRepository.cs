using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IParticipanteRepository : IGenericRepository<Participante, Guid>
    {
        Task<IEnumerable<Participante>> BuscaSemNoTraking(Expression<Func<Participante, bool>> predicate = null);

    }
}
