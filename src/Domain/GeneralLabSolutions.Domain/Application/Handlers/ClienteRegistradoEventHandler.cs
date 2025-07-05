using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ClienteRegistradoEventHandler : INotificationHandler<ClienteRegistradoEvent>
    {
        public async Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Cliente ID: {notification.Id}, Nome: {notification.Nome} registrado!");

            await Task.CompletedTask;
        }

    }
}
