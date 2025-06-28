using GeneralLabSolutions.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;

namespace VelzonModerna.Controllers
{
    public class OrcamentoController : BaseMvcController
    {
        public OrcamentoController(INotificador notificador) : base(notificador)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
