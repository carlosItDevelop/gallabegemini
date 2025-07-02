using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;
using VelzonModerna.ViewModels.ViewComponents; // Não se esqueça de adicionar o using

namespace VelzonModerna.Configuration.ViewComponents.AggregateListViewComponent
{
    [ViewComponent(Name = "AggregateList")]
    public class AggregateListViewComponent : ViewComponent
    {
        // O construtor pode permanecer vazio por enquanto, ou receber dependências se necessário no futuro.
        public AggregateListViewComponent() { }

        // O método InvokeAsync agora recebe os parâmetros
        public IViewComponentResult Invoke(
            string parentEntityType,
            Guid parentEntityId,
            Guid parentPessoaId,
            string aggregateType,
            IEnumerable<object> items)
        {
            // Criamos um modelo com todos os dados necessários para a View do componente
            var model = new AggregateListViewModel
            {
                ParentEntityType = parentEntityType,
                ParentEntityId = parentEntityId,
                ParentPessoaId = parentPessoaId,
                AggregateType = aggregateType,
                Items = items
            };

            // Passamos o modelo para a View do componente
            return View(model);
        }
    }
}