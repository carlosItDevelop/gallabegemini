using GeneralLabSolutions.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using velzon.Models;

namespace VelzonModerna.Controllers.Base
{
    public abstract class BaseMvcController : Controller
    {
        private readonly INotificador _notificador;

        protected BaseMvcController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        // *** NOVO MÉTODO ***
        // Prática menos comum, mas podemos refatorar, depois, em um único lugar
        // Como INotificador é injetado, podemos usar diretamente.
        // Isso é útil para evitar duplicação de código em cada controller.
        // Como o método Notificar é protegido na classe base, que está em outro projeto,
        // não podemos chamá-lo diretamente na controller.
        // ToDo: Refatorar para usar o mesmo método (Notificar) em todos os lugares!
        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

    }
}
