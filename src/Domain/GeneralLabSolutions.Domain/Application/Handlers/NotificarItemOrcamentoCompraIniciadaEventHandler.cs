using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class NotificarItemOrcamentoCompraIniciadaEventHandler : INotificationHandler<NotificarItemOrcamentoCompraIniciadaEvent>
    {
        public async Task Handle(NotificarItemOrcamentoCompraIniciadaEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Notificação: Compra do Item de Orçamento ID: {notification.ItemOrcamentoId} iniciada (Pedido de Compra ID: {notification.PedidoDeCompraId})!");
            await Task.CompletedTask;
        }
    }
}