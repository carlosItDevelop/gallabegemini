using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ContaRepository : GenericRepository<Conta, Guid>, IContaRepository
    {
        public ContaRepository(AppDbContext context) : base(context){}

        public async Task<IEnumerable<Conta>> ObterContasAVencer(int diasDeAntecedencia)
        {
            DateTime dataLimite = DateTime.Now.AddDays(diasDeAntecedencia);
            return await _context.Conta
                .Where(c => c.DataVencimento <= dataLimite && !c.EstaPaga && !c.Inativa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conta>> ObterContasPorTipo(TipoDeConta tipo)
        {
            return await _context.Conta
                .Where(c => c.TipoDeConta == tipo)
                .ToListAsync();
        }

        public async Task<bool> ExisteConta(Guid id)
        {
            return await _context.Conta.AnyAsync(x => x.Id == id);
        }
    }
}