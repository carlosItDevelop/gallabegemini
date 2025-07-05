using System.Linq;
using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ParticipanteRepository : GenericRepository<Participante, Guid>, IParticipanteRepository
    {
        public ParticipanteRepository(AppDbContext context) : base(context)
        {
        }

        //public async Task<IEnumerable<Participante>> BuscaSemNoTraking(Expression<Func<Participante, bool>> predicate)
        //{
        //    return await _context.Set<Participante>()
        //        //.AsNoTracking() // <== aqui ==> Retirei esta linha
        //        .Where(predicate)
        //        .ToListAsync();
        //}


        public async Task<IEnumerable<Participante>> BuscaSemNoTraking(Expression<Func<Participante, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _context.Set<Participante>().ToListAsync();
            }
            return await _context.Set<Participante>().Where(predicate).ToListAsync();
        }

    }
}
