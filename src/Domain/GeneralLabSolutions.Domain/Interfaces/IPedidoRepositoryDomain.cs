using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.SharedKernel.Enums;


namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IPedidoRepositoryDomain : IGenericRepository<Pedido, Guid>
    {
        Task<IEnumerable<Pedido>> GetAllPedidosWithIncludesAsync();

        Task<Dictionary<StatusDoPedido, List<decimal>>> GetVendasDeJaneiroADezembroDe2024Async();

    }
}
