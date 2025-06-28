// Em GeneralLabSolutions.CoreShared.Interfaces

using GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados;

namespace GeneralLabSolutions.CoreShared.Interfaces
{
    public interface IConsolidadoClienteRepository
    {
        Task<ClienteConsolidadoDto?> ObterConsolidadoClientePorIdAsync(Guid clienteId);
        Task<ItensPedidoConsolidadoDto> ObterItensPedido(Guid pedidoId);
    }
}
