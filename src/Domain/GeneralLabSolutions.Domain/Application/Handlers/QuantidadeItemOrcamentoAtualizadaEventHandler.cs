using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class QuantidadeItemOrcamentoAtualizadaEventHandler : INotificationHandler<QuantidadeItemOrcamentoAtualizadaEvent>
    {
        public async Task Handle(QuantidadeItemOrcamentoAtualizadaEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Quantidade do Item de Orçamento ID: {notification.ItemOrcamentoId} atualizada para {notification.NovaQuantidade}!");
            await Task.CompletedTask;
        }
    }
}