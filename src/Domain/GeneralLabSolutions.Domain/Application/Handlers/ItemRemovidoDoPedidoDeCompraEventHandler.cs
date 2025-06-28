using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemRemovidoDoPedidoDeCompraEventHandler : INotificationHandler<ItemRemovidoDoPedidoDeCompraEvent>
    {
        public async Task Handle(ItemRemovidoDoPedidoDeCompraEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Item ID: {notification.ItemPedidoDeCompraId} removido do Pedido de Compra ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}