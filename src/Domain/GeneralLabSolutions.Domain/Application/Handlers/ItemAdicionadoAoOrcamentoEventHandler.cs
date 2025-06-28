using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemAdicionadoAoOrcamentoEventHandler : INotificationHandler<ItemAdicionadoAoOrcamentoEvent>
    {
        public async Task Handle(ItemAdicionadoAoOrcamentoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Item ID: {notification.ProdutoId} adicionado ao Orçamento ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}