using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class OrcamentoEnviadoParaAnaliseEventHandler : INotificationHandler<OrcamentoEnviadoParaAnaliseEvent>
    {
        public async Task Handle(OrcamentoEnviadoParaAnaliseEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Orçamento ID: {notification.AggregateId} enviado para análise!");
            await Task.CompletedTask;
        }
    }
}