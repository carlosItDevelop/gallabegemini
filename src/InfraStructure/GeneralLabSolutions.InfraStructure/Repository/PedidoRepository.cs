using GeneralLabSolutions.CoreShared.DTOs.DtosGraficos;
using GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class PedidoRepository
        : GenericRepository<Pedido, Guid>,
        IPedidoRepositoryDomain,
        IPedidoRepositoryDto
    {
        public PedidoRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Pedido>> GetAllPedidosWithIncludesAsync()
        {
            var listaPedidos = await _context.Pedido
                .Include(x => x.Itens) // Incluindo ItensPedido diretamente
                    .ThenInclude(p => p.Produto)
                        .ThenInclude(f => f.Fornecedor)
                .Include(x => x.Vendedor)
                .Include(x => x.Cliente)
                // ToDo: Verificar se é necessário incluir o Fornecedor
                .AsSplitQuery() // Usando AsSplitQuery para evitar problemas de performance
                .AsNoTracking()
                .ToListAsync();

            return listaPedidos;
        }



        public async Task<PedidoResumoDto> GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido status)
        {
            var pedidos = await _context.Pedido
                .Include(x => x.Itens) // Incluindo ItensPedido diretamente
                .Where(p => p.StatusDoPedido == status)
                .ToListAsync();

            var quantidade = pedidos.Count;
            var valorTotal = pedidos.Sum(p => p.Itens.Sum(i => i.ValorUnitario * i.Quantidade));



            return new PedidoResumoDto(quantidade, valorTotal);
        }

        public async Task<Dictionary<StatusDoPedido, List<decimal>>> GetVendasDeJaneiroADezembroDe2024Async()
        {
            // Inicializando o dicionário para conter 12 meses para cada status com valores 0
            var vendasNoAno2024 = Enum.GetValues(typeof(StatusDoPedido))
                .Cast<StatusDoPedido>()
                .ToDictionary(status => status, status => new List<decimal>(new decimal [12]));

            // Obtendo os pedidos filtrados para o ano de 2024
            var pedidos = await _context.Pedido
                .Where(p => p.DataPedido.Year == 2024)
                .Include(p => p.Itens) // Inclui os itens do pedido para processamento posterior
                .ToListAsync();

            // Agrupando os pedidos por Status, Mês e somando os valores dos itens
            var result = pedidos
                .GroupBy(p => new { p.StatusDoPedido, Mes = p.DataPedido.Month })
                .Select(g => new
                {
                    Status = g.Key.StatusDoPedido,
                    Mes = g.Key.Mes,
                    Total = g.Sum(p => p.Itens.Sum(i => i.ValorUnitario * i.Quantidade))
                })
                .ToList();

            // Preencher o dicionário com os valores correspondentes para cada mês de 2024
            foreach (var item in result)
            {
                vendasNoAno2024 [item.Status] [item.Mes - 1] = item.Total; // Preenche o mês correspondente
            }

            return vendasNoAno2024;
        }


        public async Task<List<TopVendedoresDto>> GetTop10VendedoresAsync()
        {
            var topVendedores = await _context.Pedido
                .Include(p => p.Vendedor)
                .GroupBy(p => p.Vendedor.Nome)
                .Select(g => new TopVendedoresDto
                {
                    NomeVendedor = g.Key,
                    QuantidadeVendas = g.Count()
                })
                .OrderBy(v => v.NomeVendedor) // Ordenando em ordem alfabética
                .Take(10)
                .ToListAsync();

            return topVendedores;
        }

        /* 3º GRÁFICO DAQUI PRA BAIXO - Trimestral */

        public async Task<List<ClientePeriodoDto>> GetTop4ClientesPorPeriodoQuantidadeAsync(int ano, int mesesPorPeriodo)
        {
            // Garantindo que o valor de mesesPorPeriodo nunca seja zero
            if (mesesPorPeriodo <= 0)
            {
                throw new ArgumentException("O valor de meses por período deve ser maior que zero.", nameof(mesesPorPeriodo));
            }

            var query = from p in _context.Pedido
                        join i in _context.ItemPedido on p.Id equals i.PedidoId
                        join c in _context.Cliente on p.ClienteId equals c.Id
                        where p.DataPedido.Year == ano
                        group new { p, i, c } by new
                        {
                            p.ClienteId,
                            c.Nome,
                            Periodo = (p.DataPedido.Month - 1) / mesesPorPeriodo + 1 // Agrupa dinamicamente conforme o número de meses por período
                        } into g
                        select new ClientePeriodoDto
                        {
                            ClienteId = g.Key.ClienteId,
                            NomeCliente = g.Key.Nome,
                            Periodo = g.Key.Periodo,
                            QuantidadePedidos = g.Count(),
                            ValorTotal = g.Sum(x => x.i.ValorUnitario * x.i.Quantidade)
                        };



            var result = query
                .AsEnumerable()
                .GroupBy(x => x.Periodo)
                .SelectMany(g => g.OrderByDescending(c => c.QuantidadePedidos).Take(4))
                .ToList();

            return result;
        }

        public async Task<List<ClientePeriodoDto>> GetTop4ClientesPorPeriodoValorAsync(int ano, int mesesPorPeriodo)
        {
            if (mesesPorPeriodo <= 0)
            {
                throw new ArgumentException("O valor de meses por período deve ser maior que zero.", nameof(mesesPorPeriodo));
            }

            var query = from p in _context.Pedido
                        join i in _context.ItemPedido on p.Id equals i.PedidoId
                        join c in _context.Cliente on p.ClienteId equals c.Id
                        where p.DataPedido.Year == ano
                        group new { p, i, c } by new
                        {
                            p.ClienteId,
                            c.Nome,
                            Periodo = (p.DataPedido.Month - 1) / mesesPorPeriodo + 1
                        } into g
                        select new ClientePeriodoDto
                        {
                            ClienteId = g.Key.ClienteId,
                            NomeCliente = g.Key.Nome,
                            Periodo = g.Key.Periodo,
                            QuantidadePedidos = g.Count(),
                            ValorTotal = g.Sum(x => x.i.ValorUnitario * x.i.Quantidade)
                        };


            var result = query
                .AsEnumerable()
                .GroupBy(x => x.Periodo)
                .SelectMany(g => g.OrderByDescending(c => c.ValorTotal).Take(4))
                .ToList();

            return result;
        }

    }
}
