using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class PedidoDeCompraStatusAlteradoEventHandler : INotificationHandler<PedidoDeCompraStatusAlteradoEvent>
    {
        public async Task Handle(PedidoDeCompraStatusAlteradoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Status do Pedido de Compra ID: {notification.AggregateId} alterado para {notification.NovoStatus}!");
            await Task.CompletedTask;
        }
    }
}