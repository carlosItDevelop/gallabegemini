using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class TelefoneAdicionadoEventHandler : INotificationHandler<TelefoneAdicionadoEvent>
    {
        public async Task Handle(TelefoneAdicionadoEvent notification, CancellationToken cancellationToken)
        {
            // Aqui você pode implementar a lógica que deve ser executada quando um telefone é adicionado.
            // Por exemplo, você pode registrar o evento, enviar notificações, etc.
            Console.WriteLine($"Telefone Adicionado: {notification.TelefoneId}, DDD: {notification.DDD},    Numero: {notification.Numero}, Tipo: {notification.TipoDeTelefone}");

            await Task.CompletedTask;
        }
    }
}
