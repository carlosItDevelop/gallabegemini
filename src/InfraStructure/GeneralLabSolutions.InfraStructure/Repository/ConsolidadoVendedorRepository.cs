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
    public class ConsolidadoVendedorRepository
        : GenericRepository<Vendedor, Guid>,
        IConsolidadoVendedorRepositoryDomain,
        IConsolidadoVendedorRepository
    {
        private readonly IGenericRepository<Vendedor, Guid> _queryRepository;
        private readonly IGenericRepository<Pedido, Guid> _pedidoRepository;

        public ConsolidadoVendedorRepository(AppDbContext context,
                                             IGenericRepository<Vendedor, Guid> queryRepository,
                                             IGenericRepository<Pedido, Guid> pedidoRepository)
            : base(context)
        {
            _queryRepository = queryRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IEnumerable<Vendedor>> GetAllVendedoresAsync()
        {
            return await _queryRepository.GetAllAsync();
        }

        public async Task<VendedorConsolidadoDto?> ObterConsolidadoVendedorPorIdAsync(Guid vendedorId)
        {
            // Busca o vendedor pelo Id
            var vendedor = await _queryRepository.GetByIdAsync(vendedorId);

            if (vendedor == null)
                return null;

            // Busca todos os pedidos (vendas) desse vendedor, excluindo os cancelados
            var pedidosDoVendedor = await _pedidoRepository
                .SearchAsync(p => p.VendedorId == vendedor.Id && p.StatusDoPedido != StatusDoPedido.Cancelado);

            // Busca os pedidos completos (com itens e produto/categoria)
            var vendasComItens = pedidosDoVendedor
                .Select(p => _context.Pedido
                    .Include(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(prod => prod.CategoriaProduto)
                    .FirstOrDefault(x => x.Id == p.Id))
                .Where(p => p != null).ToList();

            // Encontrar o Produto mais vendido
            var produtoMaisVendidoGroup = vendasComItens
                .SelectMany(p => p.Itens)
                .GroupBy(i => i.Produto)
                .OrderByDescending(g => g.Sum(i => i.Quantidade))
                .FirstOrDefault();

            ProdutoDto? produtoMaisVendidoDto = null;
            CategoriaProdutoDto? categoriaMaisVendidaDto = null;

            if (produtoMaisVendidoGroup != null)
            {
                var produto = produtoMaisVendidoGroup.Key;
                if (produto != null)
                {
                    produtoMaisVendidoDto = new ProdutoDto
                    {
                        Id = produto.Id,
                        Nome = produto.Descricao ?? string.Empty
                        // Adicione outros campos do ProdutoDto se desejar
                    };

                    var categoria = produto.CategoriaProduto;
                    if (categoria != null)
                    {
                        categoriaMaisVendidaDto = new CategoriaProdutoDto
                        {
                            Id = categoria.Id,
                            Nome = categoria.Descricao ?? string.Empty
                        };
                    }
                }
            }

            // Monta o DTO consolidado do vendedor
            var consolidado = new VendedorConsolidadoDto
            {
                VendedorId = vendedor.Id,
                Nome = vendedor.Nome,
                TotalDeVendas = vendasComItens.Count(),
                UltimaVenda = vendasComItens.Any() ? vendasComItens.Max(p => p.DataPedido) : (DateTime?)null,
                TicketMedioPorVenda = vendasComItens.Any()
                    ? vendasComItens.Sum(p => CalcularValorTotalDaVenda(p)) / vendasComItens.Count()
                    : 0,
                IntervaloMedioEntreVendas = CalcularIntervaloMedio(vendasComItens),
                ProdutoMaisVendido = produtoMaisVendidoDto,
                CategoriaMaisVendida = categoriaMaisVendidaDto,
                QuantidadeDeVendasPorProduto = produtoMaisVendidoGroup?.Sum(i => i.Quantidade) ?? 0,
                QuantidadeDeVendasPorCategoria = produtoMaisVendidoGroup?.Sum(i => i.Quantidade) ?? 0,
                ValorTotalDeVendas = vendasComItens.Sum(v => CalcularValorTotalDaVenda(v)),
                HistoricoDeVendas = vendasComItens.Select(p => new VendaHistoricoDto
                {
                    Id = p.Id,
                    DataVenda = p.DataPedido,
                    ValorTotal = CalcularValorTotalDaVenda(p),
                    Status = p.StatusDoPedido.ToString()
                }).ToList()
                // Complete os demais campos do DTO se desejar
            };

            return consolidado;
        }


        public async Task<ItensVendaConsolidadoDto> ObterItensVenda(Guid vendaId)
        {
            var venda = await _context.Pedido
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == vendaId);

            if (venda == null)
            {
                // Retorna DTO vazio ao invés de null, evita null reference nas camadas superiores
                return new ItensVendaConsolidadoDto();
            }

            return new ItensVendaConsolidadoDto
            {
                Itens = venda.Itens.Select(item => new ItemVendaDto
                {
                    NomeProduto = item.Produto.Descricao,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                }).ToList(),
                QuantidadeTotalItens = venda.Itens.Sum(i => i.Quantidade),
                ValorTotalItens = venda.Itens.Sum(i => i.ValorUnitario * i.Quantidade)
            };
        }



        // Método para calcular o valor total de uma venda com base nos itens
        private decimal CalcularValorTotalDaVenda(Pedido venda)
        {
            return venda.Itens.Sum(i => i.ValorUnitario * i.Quantidade);
        }

        private int CalcularIntervaloMedio(IEnumerable<Pedido> vendas)
        {
            if (!vendas.Any())
                return 0;

            var vendasOrdenadas = vendas.OrderBy(p => p.DataPedido).ToList();
            var intervalos = new List<int>();

            for (int i = 1; i < vendasOrdenadas.Count; i++)
            {
                var intervalo = (vendasOrdenadas [i].DataPedido - vendasOrdenadas [i - 1].DataPedido).Days;
                intervalos.Add(intervalo);
            }

            return intervalos.Any() ? (int)intervalos.Average() : 0;
        }
    }
}
