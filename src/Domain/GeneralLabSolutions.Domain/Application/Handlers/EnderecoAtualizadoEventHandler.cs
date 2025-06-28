using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class EnderecoAtualizadoEventHandler : INotificationHandler<EnderecoAtualizadoEvent>
    {
        public async Task Handle(EnderecoAtualizadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Endereço ID: {notification.EnderecoId} atualizado para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}