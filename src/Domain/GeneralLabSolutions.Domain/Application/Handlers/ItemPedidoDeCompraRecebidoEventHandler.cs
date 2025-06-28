using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemPedidoDeCompraRecebidoEventHandler : INotificationHandler<ItemPedidoDeCompraRecebidoEvent>
    {
        public async Task Handle(ItemPedidoDeCompraRecebidoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Item ID: {notification.ItemPedidoDeCompraId} do Pedido de Compra ID: {notification.AggregateId} recebido!");
            await Task.CompletedTask;
        }
    }
}