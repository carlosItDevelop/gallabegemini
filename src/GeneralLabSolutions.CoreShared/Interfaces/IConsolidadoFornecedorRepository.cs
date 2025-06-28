// Em GeneralLabSolutions.CoreShared.Interfaces

using GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados;

namespace GeneralLabSolutions.CoreShared.Interfaces
{
    public interface IConsolidadoFornecedorRepository
    {
        Task<FornecedorConsolidadoDto?> ObterConsolidadoFornecedorPorIdAsync(Guid fornecedorId);
    }
}
