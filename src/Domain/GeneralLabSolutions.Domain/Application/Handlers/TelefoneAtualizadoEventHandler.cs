using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class TelefoneAtualizadoEventHandler : INotificationHandler<TelefoneAtualizadoEvent>
    {
        public async Task Handle(TelefoneAtualizadoEvent notification, CancellationToken cancellationToken)
        {
            // Aqui você pode implementar a lógica que deve ser executada quando um telefone é atualizado.
            // Por exemplo, você pode registrar o evento, enviar notificações, etc.
            Console.WriteLine($"Telefone Atualizado: {notification.TelefoneId}, DDD: {notification.DDD}, Numero: {notification.Numero}, Tipo: {notification.TipoDeTelefone}");
            // Você pode acessar o AggregateId para saber a qual Cliente/Fornecedor/Vendedor este telefone pertence
            // Por exemplo, se você quiser registrar o ID do agregado:
            Console.WriteLine($"Pertence ao agregado com ID: {notification.AggregateId}");
            // Se necessário, você pode realizar outras operações, como atualizar registros no banco de dados,
            // enviar notificações, etc.

            await Task.CompletedTask; // Simula uma operação assíncrona
        }
    }
}
