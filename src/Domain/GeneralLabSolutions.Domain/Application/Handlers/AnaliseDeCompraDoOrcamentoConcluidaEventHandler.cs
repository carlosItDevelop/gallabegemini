using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class AnaliseDeCompraDoOrcamentoConcluidaEventHandler : INotificationHandler<AnaliseDeCompraDoOrcamentoConcluidaEvent>
    {
        public async Task Handle(AnaliseDeCompraDoOrcamentoConcluidaEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Análise de compra do Orçamento ID: {notification.AggregateId} concluída!");
            await Task.CompletedTask;
        }
    }
}