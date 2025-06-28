using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ItemOrcamentoStatusAlteradoEventHandler : INotificationHandler<ItemOrcamentoStatusAlteradoEvent>
    {
        public async Task Handle(ItemOrcamentoStatusAlteradoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Status do Item de Orçamento ID: {notification.ItemOrcamentoId} alterado para {notification.NovoStatus}!");
            await Task.CompletedTask;
        }
    }
}