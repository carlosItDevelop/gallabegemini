using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class DadosBancariosAtualizadosEventHandler : INotificationHandler<DadosBancariosAtualizadosEvent>
    {
        public async Task Handle(DadosBancariosAtualizadosEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Dados Bancários ID: {notification.DadosBancariosId} atualizados para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}