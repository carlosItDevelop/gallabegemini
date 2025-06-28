using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class DadosBancariosRemovidosEvent : DomainEvent
    {
        public Guid DadosBancariosId { get; private set; }

        // AggregateId aqui será o Cliente.Id
        public DadosBancariosRemovidosEvent(Guid clienteId, Guid dadosBancariosId)
            : base(clienteId)
        {
            DadosBancariosId = dadosBancariosId;
        }
    }
}