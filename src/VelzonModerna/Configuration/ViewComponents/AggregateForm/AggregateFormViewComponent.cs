using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels.ViewComponents;

namespace VelzonModerna.Configuration.ViewComponents.AggregateForm
{
    [ViewComponent(Name = "AggregateForm")]
    public class AggregateFormViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string parentEntityType, object parentViewModel, string aggregateType)
        {
            // Usando reflexão para obter os IDs do ViewModel pai (ClienteViewModel, FornecedorViewModel, etc.)
            var parentEntityId = (Guid)parentViewModel.GetType().GetProperty("Id").GetValue(parentViewModel, null);
            var parentPessoaId = (Guid)parentViewModel.GetType().GetProperty("PessoaId").GetValue(parentViewModel, null);

            var model = new AggregateFormViewModel
            {
                ParentEntityType = parentEntityType,
                ParentEntityId = parentEntityId,
                ParentPessoaId = parentPessoaId,
                AggregateType = aggregateType,
                // O caminho para a partial de campos é definido por convenção
                FieldsPartialViewPath = $"~/Views/Shared/PartialViews/AggregateFields/_{aggregateType}Fields.cshtml"
            };

            return View(model);
        }
    }
}
