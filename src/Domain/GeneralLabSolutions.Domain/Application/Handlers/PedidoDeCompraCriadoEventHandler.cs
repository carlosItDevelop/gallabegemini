using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class PedidoDeCompraCriadoEventHandler : INotificationHandler<PedidoDeCompraCriadoEvent>
    {
        public async Task Handle(PedidoDeCompraCriadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Pedido de Compra ID: {notification.AggregateId} criado para o Fornecedor ID: {notification.FornecedorId}!");
            await Task.CompletedTask;
        }
    }
}