// ContaPagaEventHandler.cs
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ContaPagaEventHandler : INotificationHandler<ContaPagaEvent>
    {
        public async Task Handle(ContaPagaEvent notification, CancellationToken cancellationToken)
        {
            // Lógica para lidar com o evento ContaPagaEvent
            // Ex: Enviar notificação, atualizar status em outro sistema, etc.
            Console.WriteLine($"Conta ID: {notification.ContaId} foi paga!");
            await Task.CompletedTask;
        }
    }
}

// Crie o handler para ContaInativadaEvent, se tiver criado o evento.