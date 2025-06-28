using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemAdicionadoAoPedidoDeCompraEventHandler : INotificationHandler<ItemAdicionadoAoPedidoDeCompraEvent>
    {
        public async Task Handle(ItemAdicionadoAoPedidoDeCompraEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Item ID: {notification.ItemPedidoDeCompraId} adicionado ao Pedido de Compra ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}