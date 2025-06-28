using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IContaRepository : IGenericRepository<Conta, Guid>
    {
        // Métodos específicos para Conta, se necessário
        Task<IEnumerable<Conta>> ObterContasAVencer(int diasDeAntecedencia);
        Task<IEnumerable<Conta>> ObterContasPorTipo(TipoDeConta tipo); //Exemplo
        Task<bool> ExisteConta(Guid id);
    }
}