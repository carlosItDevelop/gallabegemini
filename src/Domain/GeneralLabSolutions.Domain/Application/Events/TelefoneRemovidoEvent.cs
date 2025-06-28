using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class TelefoneRemovidoEvent : DomainEvent
    {
        public Guid TelefoneId { get; private set; }

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor ao qual o telefone pertencia
        public TelefoneRemovidoEvent(Guid aggregateId, Guid telefoneId)
            : base(aggregateId)
        {
            TelefoneId = telefoneId;
        }
    }
}