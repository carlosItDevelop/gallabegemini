using GeneralLabSolutions.CoreShared.DTOs;
using GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents;
using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Configuration.ViewComponents.CardResumoPedido
{
    [ViewComponent(Name = "cardResumoPedido")]
    public class CardResumoPedidoViewComponents : ViewComponent
    {
        private readonly IPedidoRepositoryDto _pedidoRepository;

        public CardResumoPedidoViewComponents(IPedidoRepositoryDto pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        // Propriedades que correspondem aos parâmetros que você deseja passar
        public string Titulo { get; set; } = string.Empty;
        public string CssColor { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Link { get; set; }
        public StatusDoPedido Status { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(StatusDoPedido status, string titulo, string cssColor, string? icon = null, string? link = null)
        {
            // Armazena os parâmetros nas propriedades da classe
            Titulo = titulo;
            CssColor = cssColor;
            Icon = icon;
            Link = link;
            Status = (StatusDoPedido)status;

            // Recupera os dados do repositório
            var resumoTotalValor = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(Status);

            var valorFormatado = resumoTotalValor.ValorTotal.ToString("F2", CultureInfo.InvariantCulture); // Formata com ponto como separador decimal


            // Cria o modelo para a View
            var model = new CardPedidoViewModel()
            {
                Titulo = Titulo,
                Quantidade = resumoTotalValor.Quantidade,
                Valor = valorFormatado,
                CssColor = CssColor,
                Icon = Icon,
                Link = Link
            };

            return View(model);
        }

    }
}
