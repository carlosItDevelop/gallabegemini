using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class EnderecoAdicionadoEventHandler : INotificationHandler<EnderecoAdicionadoEvent>
    {
        public async Task Handle(EnderecoAdicionadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Endereço ID: {notification.EnderecoId} adicionado para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}