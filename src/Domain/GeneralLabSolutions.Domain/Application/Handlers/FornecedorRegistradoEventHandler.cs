using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class FornecedorRegistradoEventHandler : INotificationHandler<FornecedorRegistradoEvent>
    {
        public async Task Handle(FornecedorRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Fornecedor ID: {notification.Id}, Nome: {notification.Nome} registrado!");

            await Task.CompletedTask;
        }
    }
}
