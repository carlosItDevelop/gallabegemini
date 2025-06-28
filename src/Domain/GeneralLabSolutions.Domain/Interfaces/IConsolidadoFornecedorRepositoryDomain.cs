// Em GeneralLabSolutions.Domain.Interfaces

using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IConsolidadoFornecedorRepositoryDomain
    {
        Task<IEnumerable<Fornecedor>> ObterTodosFornecedoresAsync();
    }
}
