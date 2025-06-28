using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class NotificarItemOrcamentoCompraCanceladaEventHandler : INotificationHandler<NotificarItemOrcamentoCompraCanceladaEvent>
    {
        public async Task Handle(NotificarItemOrcamentoCompraCanceladaEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Notificação: Compra do Item de Orçamento ID: {notification.ItemOrcamentoId} cancelada (Pedido de Compra ID: {notification.PedidoDeCompraId})!");
            await Task.CompletedTask;
        }
    }
}