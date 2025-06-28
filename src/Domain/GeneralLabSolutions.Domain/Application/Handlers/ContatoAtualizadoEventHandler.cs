using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ContatoAtualizadoEventHandler : INotificationHandler<ContatoAtualizadoEvent>
    {
        public async Task Handle(ContatoAtualizadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Contato ID: {notification.ContatoId}, Nome: {notification.Nome} atualizado para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}