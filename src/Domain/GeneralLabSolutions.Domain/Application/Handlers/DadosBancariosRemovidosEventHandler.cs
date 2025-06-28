using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class DadosBancariosRemovidosEventHandler : INotificationHandler<DadosBancariosRemovidosEvent>
    {
        public async Task Handle(DadosBancariosRemovidosEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Dados Bancários ID: {notification.DadosBancariosId} removidos do Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}