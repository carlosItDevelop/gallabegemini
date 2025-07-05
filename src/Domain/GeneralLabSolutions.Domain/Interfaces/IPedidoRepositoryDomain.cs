using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;


namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IPedidoRepositoryDomain : IGenericRepository<Pedido, Guid>
    {
        Task<IEnumerable<Pedido>> GetAllPedidosWithIncludesAsync();

        Task<Dictionary<StatusDoPedido, List<decimal>>> GetVendasDeJaneiroADezembroDe2024Async();

    }
}
