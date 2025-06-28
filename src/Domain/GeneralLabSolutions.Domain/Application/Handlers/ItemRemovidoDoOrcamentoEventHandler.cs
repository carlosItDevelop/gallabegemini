using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemRemovidoDoOrcamentoEventHandler : INotificationHandler<ItemRemovidoDoOrcamentoEvent>
    {
        public async Task Handle(ItemRemovidoDoOrcamentoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Item ID: {notification.ItemOrcamentoId} removido do Orçamento ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}