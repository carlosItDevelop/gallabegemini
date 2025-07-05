using GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ConsolidadoClienteRepository
        : GenericRepository<Cliente, Guid>,
        IConsolidadoClienteRepositoryDomain,
        IConsolidadoClienteRepository
    {
        private readonly IGenericRepository<Cliente, Guid> _queryRepository;
        private readonly IGenericRepository<Pedido, Guid> _pedidoRepository;

        public ConsolidadoClienteRepository(AppDbContext context,
            IGenericRepository<Cliente, Guid> queryRepository,
            IGenericRepository<Pedido, Guid> pedidoRepository)
            : base(context)
        {
            _queryRepository = queryRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _queryRepository.GetAllAsync();
        }

        // Método principal que retorna o consolidado de um cliente específico
        public async Task<ClienteConsolidadoDto?> ObterConsolidadoClientePorIdAsync(Guid clienteId)
        {
            var cliente = await _queryRepository.GetByIdAsync(clienteId);

            if (cliente is null)
                return null;

            var pedidosDoCliente = await _pedidoRepository
                .SearchAsync(p => p.ClienteId == cliente.Id && p.StatusDoPedido != StatusDoPedido.Cancelado);

            var pedidosComItens = pedidosDoCliente
                .Select(p => _context.Pedido
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(prod => prod.CategoriaProduto)
                    .FirstOrDefault(x => x.Id == p.Id))
                .Where(p => p != null).ToList();

            // Calcula produto/categoria mais comprada
            var produtoMaisComprado = pedidosComItens
                .SelectMany(p => p.Itens)
                .GroupBy(i => i.ProdutoId)
                .OrderByDescending(g => g.Sum(i => i.Quantidade))
                .FirstOrDefault();

            ProdutoDto? produtoMaisCompradoDto = null;
            CategoriaProdutoDto? categoriaMaisCompradaDto = null;
            if (produtoMaisComprado != null)
            {
                var produto = produtoMaisComprado.First().Produto;
                produtoMaisCompradoDto = new ProdutoDto
                {
                    Id = produto.Id,
                    Nome = produto.Descricao
                    // Preencha mais campos se necessário
                };

                var categoria = produto.CategoriaProduto;
                if (categoria != null)
                {
                    categoriaMaisCompradaDto = new CategoriaProdutoDto
                    {
                        Id = categoria.Id,
                        Nome = categoria.Descricao
                        // Preencha mais campos se necessário
                    };
                }
            }

            var consolidado = new ClienteConsolidadoDto
            {
                ClienteId = cliente.Id,
                Nome = cliente.Nome,
                QuantidadeDePedidos = pedidosComItens.Count(),
                UltimaCompraDesteCliente = pedidosComItens.Any() ? pedidosComItens.Max(p => p.DataPedido) : (DateTime?)null,
                TicketMedioPorPedido = pedidosComItens.Any() ? pedidosComItens.Sum(p => CalcularValorTotalDoPedido(p)) / pedidosComItens.Count() : 0,
                IntervaloMedioEntrePedidos = CalcularIntervaloMedio(pedidosComItens),
                HistoricoDePedidos = pedidosComItens.Select(p => new PedidoHistoricoDto
                {
                    Id = p.Id,
                    DataPedido = p.DataPedido,
                    ValorTotal = CalcularValorTotalDoPedido(p),
                    Status = p.StatusDoPedido.ToString()
                }).ToList(),
                ValorTotalDeCompras = pedidosComItens.Sum(p => CalcularValorTotalDoPedido(p)),
                ProdutoMaisComprado = produtoMaisCompradoDto,
                CategoriaMaisComprada = categoriaMaisCompradaDto
            };

            return consolidado;
        }


        public async Task<Cliente?> ObterClienteComPedidosEItensEProdutoEFornecedor(Guid clienteId)
        {
            var cliente = await _context.Cliente
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(prod => prod.Fornecedor) // Inclui os dados do fornecedor
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            return cliente;
        }


        public async Task<Pedido?> ObterPedidoPorClienteIdComItensEDadosDoFornecedor(Guid clienteId)
        {
            var pedido = await _context.Pedido
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(prod => prod.Fornecedor) // Inclui os dados do fornecedor
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            return pedido;
        }


        public async Task<ItensPedidoConsolidadoDto> ObterItensPedido(Guid pedidoId)
        {
            var pedido = await ObterPedidoPorClienteIdComItensEDadosDoFornecedor(pedidoId);

            if (pedido == null)
            {
                return new ItensPedidoConsolidadoDto();
            }

            return new ItensPedidoConsolidadoDto
            {
                Itens = pedido.Itens.Select(item => new ItemPedidoDto
                {
                    NomeProduto = item.Produto.Descricao,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                }).ToList(),
                QuantidadeTotalItens = pedido.Itens.Sum(i => i.Quantidade),
                ValorTotalItens = pedido.Itens.Sum(i => i.ValorUnitario * i.Quantidade)
            };
        }



        // Método para calcular o valor total de um pedido com base nos itens
        private decimal CalcularValorTotalDoPedido(Pedido pedido)
        {
            return pedido.Itens.Sum(i => i.ValorUnitario * i.Quantidade);
        }

        // Método para calcular o intervalo médio entre pedidos
        private int CalcularIntervaloMedio(IEnumerable<Pedido> pedidos)
        {
            if (!pedidos.Any())
                return 0;

            var intervalos = pedidos.OrderBy(p => p.DataPedido)
                                    .Select((p, i) => i > 0 ? (p.DataPedido - pedidos.ElementAt(i - 1).DataPedido).Days : 0)
                                    .Skip(1)
                                    .ToList();

            return intervalos.Any() ? (int)intervalos.Average() : 0;
        }
    }
}
