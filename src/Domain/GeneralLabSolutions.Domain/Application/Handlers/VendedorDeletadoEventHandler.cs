using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class VendedorDeletadoEventHandler : INotificationHandler<VendedorDeletadoEvent>
    {
        public async Task Handle(VendedorDeletadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Vendedor ID: {notification.Id} registrado!");

            await Task.CompletedTask;
        }
    }
}
