using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class DadosBancariosAdicionadosEventHandler : INotificationHandler<DadosBancariosAdicionadosEvent>
    {
        public async Task Handle(DadosBancariosAdicionadosEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Dados Bancários ID: {notification.DadosBancariosId} adicionados para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}