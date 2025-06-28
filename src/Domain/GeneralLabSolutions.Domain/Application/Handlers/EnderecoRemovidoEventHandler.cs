using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class EnderecoRemovidoEventHandler : INotificationHandler<EnderecoRemovidoEvent>
    {
        public async Task Handle(EnderecoRemovidoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Endereço ID: {notification.EnderecoId} removido do Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}