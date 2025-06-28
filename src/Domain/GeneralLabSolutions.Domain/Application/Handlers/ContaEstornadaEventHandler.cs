// ContaEstornadaEventHandler.cs
using GeneralLabSolutions.Domain.Application.Events;
using MediatR;


namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ContaEstornadaEventHandler : INotificationHandler<ContaEstornadaEvent>
    {
        public async Task Handle(ContaEstornadaEvent notification, CancellationToken cancellationToken)
        {
            //Lógica do estorno!
            Console.WriteLine($"Conta ID: {notification.ContaId} foi Estornada!");
            await Task.CompletedTask;
        }
    }
}

// Crie o handler para ContaInativadaEvent, se tiver criado o evento.