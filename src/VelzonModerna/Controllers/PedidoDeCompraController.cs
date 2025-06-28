using GeneralLabSolutions.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;

namespace VelzonModerna.Controllers
{
    public class PedidoDeCompraController : BaseMvcController
    {
        public PedidoDeCompraController(INotificador notificador) : base(notificador)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
