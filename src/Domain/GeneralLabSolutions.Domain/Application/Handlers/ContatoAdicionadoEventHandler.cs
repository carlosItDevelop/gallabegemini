using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ContatoAdicionadoEventHandler : INotificationHandler<ContatoAdicionadoEvent>
    {
        public async Task Handle(ContatoAdicionadoEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Contato ID: {notification.ContatoId}, Nome: {notification.Nome} adicionado para o Cliente/Fornecedor ID: {notification.AggregateId}!");
            await Task.CompletedTask;
        }
    }
}