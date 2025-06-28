// Em GeneralLabSolutions.Domain.Interfaces

using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IConsolidadoClienteRepositoryDomain
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> ObterClienteComPedidosEItensEProdutoEFornecedor(Guid clienteId);
        Task<Pedido?> ObterPedidoPorClienteIdComItensEDadosDoFornecedor(Guid clienteId);
    }
}
