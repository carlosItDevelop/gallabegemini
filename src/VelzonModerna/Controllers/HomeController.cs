using GeneralLabSolutions.CoreShared.DTOs.DtosGraficos;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    /// <summary>
    /// DashBoard do Projeto MVC (VelzonModerna)
    /// Verificar a melhor maneira de mudar o nome
    /// desta classe para algo mais apropriado.
    /// E destacar os impactos no projeto.
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class HomeController : Controller
    {

        private readonly IPedidoRepositoryDto _pedidoRepository;
        private readonly IPedidoRepositoryDomain _pedidoRepositoryDomain;


        public HomeController(IPedidoRepositoryDto pedidoRepository, IPedidoRepositoryDomain pedidoRepositoryDomain)
        {
            _pedidoRepository = pedidoRepository;
            _pedidoRepositoryDomain = pedidoRepositoryDomain;
        }

        #region: Obtem as vendas do ano para gráfico
        [HttpGet]
        public async Task<IActionResult> GetVendasNoAnoData()
        {
            try
            {
                var vendasNoAno = await _pedidoRepositoryDomain.GetVendasDeJaneiroADezembroDe2024Async();

                var result = new VendasNoAnoDto
                {
                    Pagos = vendasNoAno [StatusDoPedido.Pago],
                    EmProcessamento = vendasNoAno [StatusDoPedido.EmProcessamento],
                    Cancelados = vendasNoAno [StatusDoPedido.Cancelado]
                };

                return Json(result);

            } catch (Exception ex)
            {
                // Log the error (you might want to log it to a file or database)
                Console.WriteLine(ex.Message);
                // Return a server error status
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion


        #region: Obtem os melhores vendedores
        [HttpGet]
        public async Task<IActionResult> GetTop10VendedoresData()
        {
            try
            {
                var topVendedores = await _pedidoRepository.GetTop10VendedoresAsync();
                return Json(topVendedores);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion


        /* 3º GRÁFICO DAQUI PRA BAIXO - Trimestral */

        #region: GetTop4ClientesPorPeriodoQuantidadeData(int? ano, int? periodo)
        [HttpGet]
        public async Task<IActionResult> GetTop4ClientesPorPeriodoQuantidadeData(int? ano, int? periodo) // ERROR: estava passando: "mesesPorPeriodo"
        {
            try
            {

                // Garantir que valores padrão sejam usados se não forem passados
                int anoUsado = ano ?? 2024;
                int mesesPorPeriodoUsado = periodo ?? 6; // Semestral por padrão

                var topClientesQuantidade = await _pedidoRepository.GetTop4ClientesPorPeriodoQuantidadeAsync(anoUsado, mesesPorPeriodoUsado);


                var clientesAgrupadosPorPeriodo = topClientesQuantidade
                    .GroupBy(p => p.Periodo)
                    .Select(grupo => new
                    {
                        Periodo = grupo.Key,
                        Clientes = grupo.Select(cliente => new
                        {
                            NomeCliente = cliente.NomeCliente,
                            QuantidadePedidos = cliente.QuantidadePedidos
                        }).ToList()
                    })
                    .OrderBy(g => g.Periodo)
                    .ToList();

                //string jsonOutput = JsonSerializer.Serialize(clientesAgrupadosPorPeriodo, new JsonSerializerOptions { WriteIndented = true });
                //Console.WriteLine(jsonOutput);

                return Json(clientesAgrupadosPorPeriodo);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion


        #region: GetTop4ClientesPorPeriodoValorData(int? ano, int? periodo)
        [HttpGet]
        public async Task<IActionResult> GetTop4ClientesPorPeriodoValorData(int? ano, int? periodo) // ERROR: estava passando: "mesesPorPeriodo"
        {
            try
            {

                // Garantir que valores padrão sejam usados se não forem passados
                int anoUsado = ano ?? 2024;
                int mesesPorPeriodoUsado = periodo ?? 4; // Quadrimestral por padrão

                var topClientesValor = await _pedidoRepository.GetTop4ClientesPorPeriodoValorAsync(anoUsado, mesesPorPeriodoUsado);

                var clientesAgrupadosPorPeriodo = topClientesValor
                    .GroupBy(p => p.Periodo)
                    .Select(grupo => new
                    {
                        Periodo = grupo.Key,
                        Clientes = grupo.Select(cliente => new
                        {
                            NomeCliente = cliente.NomeCliente,
                            ValorTotal = cliente.ValorTotal
                        }).ToList()
                    })
                    .OrderBy(g => g.Periodo)
                    .ToList();

                //string jsonOutput = JsonSerializer.Serialize(clientesAgrupadosPorPeriodo, new JsonSerializerOptions { WriteIndented = true });
                //Console.WriteLine(jsonOutput);

                return Json(clientesAgrupadosPorPeriodo);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region: Exemplo de Serialização para Json
        //// Serializar o objeto para JSON e imprimir no console
        //string jsonOutput = JsonSerializer.Serialize(clientesAgrupadosPorTrimestre, new JsonSerializerOptions { WriteIndented = true });
        //Console.WriteLine(jsonOutput);
        #endregion



        [HttpGet]
        public async Task<IActionResult> GetEstadosPedidoDonutData()
        {
            try
            {

                var ValorEQuantidadeOrçamento = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Orcamento);
                var ValorEQuantidadeEmprocessamento = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.EmProcessamento);
                var ValorEQuantidadeCancelado = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Cancelado);
                var ValorEQuantidadeEntregue = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Entregue);

                var model = new DonutEstadosPedidosViewModel()
                {
                    QuantidadeOrcamento = ValorEQuantidadeOrçamento.Quantidade,
                    QuantidadeEmProcessamento = ValorEQuantidadeEmprocessamento.Quantidade,
                    QuantidadeCancelado = ValorEQuantidadeCancelado.Quantidade,
                    QuantidadeEntregue = ValorEQuantidadeEntregue.Quantidade,

                    ValorOrcamento = ValorEQuantidadeOrçamento.ValorTotal.ToString("F2", CultureInfo.InvariantCulture), // Formata com ponto como separador decimal,
                    ValorEmProcessamento = ValorEQuantidadeEmprocessamento.ValorTotal.ToString("F2", CultureInfo.InvariantCulture),  // Formata com ponto como separador decimal,
                    ValorCancelado = ValorEQuantidadeCancelado.ValorTotal.ToString("F2", CultureInfo.InvariantCulture), // Formata com ponto como separador decimal,
                    ValorEntregue = ValorEQuantidadeEntregue.ValorTotal.ToString("F2", CultureInfo.InvariantCulture) // Formata com ponto como separador decimal
                };

                return Json(model);


            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
