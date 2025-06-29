using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class VendedorRegistradoEventHandler : INotificationHandler<VendedorRegistradoEvent>
    {
        public async Task Handle(VendedorRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Vendedor ID: {notification.Id}, Nome: {notification.Nome} registrado!");

            await Task.CompletedTask;
        }
    }
}
