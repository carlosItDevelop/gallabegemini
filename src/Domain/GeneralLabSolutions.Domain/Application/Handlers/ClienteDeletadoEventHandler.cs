using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ClienteDeletadoEventHandler : INotificationHandler<ClienteDeletadoEvent>
    {
        public async Task Handle(ClienteDeletadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Cliente ID: {notification.Id}, Nome: {notification.Nome} excluído!");

            await Task.CompletedTask;
        }
    }
}
