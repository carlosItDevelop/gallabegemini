using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class OrcamentoCriadoEventHandler : INotificationHandler<OrcamentoCriadoEvent>
    {
        public async Task Handle(OrcamentoCriadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Orçamento ID: {notification.AggregateId} criado para o Cliente ID: {notification.NomeClientePotencial}!");
            await Task.CompletedTask;
        }
    }
}