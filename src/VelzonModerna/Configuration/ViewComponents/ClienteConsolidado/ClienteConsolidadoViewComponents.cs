using GeneralLabSolutions.CoreShared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Configuration.ViewComponents.ClienteConsolidado
{
    [ViewComponent(Name = "clienteConsolidado")]
    public class ClienteConsolidadoViewComponents : ViewComponent
    {
        private readonly IConsolidadoClienteRepository _consolidadoClienteRepository;
        public ClienteConsolidadoViewComponents(
                IConsolidadoClienteRepository consolidadoClienteRepository
            )
        {
            _consolidadoClienteRepository = consolidadoClienteRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid clienteId)
        {
            var consolidado = await _consolidadoClienteRepository.ObterConsolidadoClientePorIdAsync(clienteId);

            return View(consolidado);
        }
    }
}
