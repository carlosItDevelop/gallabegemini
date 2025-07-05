using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Configuration.ViewComponents.DonutEstadosPedidos
{
    [ViewComponent(Name = "donutEstadosPedidos")]
    public class DonutEstadosPedidosViewComponent : ViewComponent
    {
        private readonly IPedidoRepositoryDto _pedidoRepository;

        public DonutEstadosPedidosViewComponent(IPedidoRepositoryDto pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        // Propriedades que correspondem aos parâmetros que você deseja passar
        public int QuantidadeOrçamento { get; set; } = 0;
        public int QuantidadeEmProcessamento { get; set; } = 0;
        public int QuantidadeCancelado { get; set; } = 0;
        public int QuantidadeEntregue { get; set; } = 0;

        public decimal ValorOrçamento { get; set; } = 0;
        public decimal ValorEmProcessamento { get; set; } = 0;
        public decimal ValorCancelado { get; set; } = 0;
        public decimal ValorEntregue { get; set; } = 0;


        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Armazena os parâmetros nas propriedades da classe

            // Recupera os dados do repositório
            var ValorEQuantidadeOrcamento = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Orcamento);
            var ValorEQuantidadeEmProcessamento = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.EmProcessamento);
            var ValorEQuantidadeCancelado = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Cancelado);
            var ValorEQuantidadeEntregue = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Entregue);


            // Cria o modelo para a View
            var model = new DonutEstadosPedidosViewModel()
            {
                QuantidadeOrcamento = ValorEQuantidadeOrcamento.Quantidade,
                QuantidadeEmProcessamento = ValorEQuantidadeEmProcessamento.Quantidade,
                QuantidadeCancelado = ValorEQuantidadeCancelado.Quantidade,
                QuantidadeEntregue = ValorEQuantidadeEntregue.Quantidade,

                ValorOrcamento = ValorEQuantidadeOrcamento.ValorTotal.ToString("F2", CultureInfo.InvariantCulture),// Formata com ponto como separador decimal
                ValorEmProcessamento = ValorEQuantidadeEmProcessamento.ValorTotal.ToString("F2", CultureInfo.InvariantCulture), // Formata com ponto como separador decimal
                ValorCancelado = ValorEQuantidadeCancelado.ValorTotal.ToString("F2", CultureInfo.InvariantCulture), // Formata com ponto como separador decimal
                ValorEntregue = ValorEQuantidadeEntregue.ValorTotal.ToString("F2", CultureInfo.InvariantCulture) // Formata com ponto como separador decimal
            };

            return View(await Task.FromResult(model));
        }

    }
}