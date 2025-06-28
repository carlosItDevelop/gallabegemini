using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ClienteAtualizadoEventHandler : INotificationHandler<ClienteAtualizadoEvent>
    {
        public async Task Handle(ClienteAtualizadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Cliente ID: {notification.Id}, Nome: {notification.Nome} Atualizado!");

            await Task.CompletedTask;
        }
    }
}
