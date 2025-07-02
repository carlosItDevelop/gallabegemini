using GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents;
using GeneralLabSolutions.InfraStructure.Repository;
using GeneralLabSolutions.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Configuration.ViewComponents.AggregateListViewComponent
{
    [ViewComponent(Name = "aggregateList")]
    public class AggregateListViewComponent : ViewComponent
    {

        public AggregateListViewComponent()
        {
                
        }

        public async Task<IViewComponentResult> InvokeAsync(StatusDoPedido status, string titulo, string cssColor, string? icon = null, string? link = null)
        {

            var model = new CardPedidoViewModel()
            {
                Titulo = "",
                Quantidade = 0,
                Valor = "0",
                CssColor = "",
                Icon = "",
                Link = ""
            };

            return View(await Task.FromResult(model));
        }
    }
}
