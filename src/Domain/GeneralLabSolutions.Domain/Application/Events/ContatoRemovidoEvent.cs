using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ContatoRemovidoEvent : DomainEvent
    {
        public Guid ContatoId { get; private set; }

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor ao qual o contato pertencia
        public ContatoRemovidoEvent(Guid aggregateId, Guid contatoId)
            : base(aggregateId)
        {
            ContatoId = contatoId;
        }
    }
}