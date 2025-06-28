using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class NotificarItemOrcamentoCompraConcluidaEventHandler : INotificationHandler<NotificarItemOrcamentoCompraConcluidaEvent>
    {
        public async Task Handle(NotificarItemOrcamentoCompraConcluidaEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Notificação: Compra do Item de Orçamento ID: {notification.ItemOrcamentoId} concluída (Pedido de Compra ID: {notification.PedidoDeCompraId})!");
            await Task.CompletedTask;
        }
    }
}