using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class FornecedorDeletadoEventHandler : INotificationHandler<FornecedorDeletadoEvent>
    {
        public async Task Handle(FornecedorDeletadoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Fornecedor ID: {notification.Id}, Nome: {notification.Nome} excluído!");

            await Task.CompletedTask;
        }
    }
}
