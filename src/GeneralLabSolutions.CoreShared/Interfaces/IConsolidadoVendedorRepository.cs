// Em GeneralLabSolutions.CoreShared.Interfaces

using GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados;

namespace GeneralLabSolutions.CoreShared.Interfaces
{
    public interface IConsolidadoVendedorRepository
    {
        Task<VendedorConsolidadoDto?> ObterConsolidadoVendedorPorIdAsync(Guid vendedorId);
        Task<ItensVendaConsolidadoDto> ObterItensVenda(Guid vendaId);
    }
}
