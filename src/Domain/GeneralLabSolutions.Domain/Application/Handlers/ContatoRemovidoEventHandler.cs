using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ContatoRemovidoEventHandler : INotificationHandler<ContatoRemovidoEvent>
    {
        public async Task Handle(ContatoRemovidoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Contato ID: {notification.ContatoId} removido do Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}