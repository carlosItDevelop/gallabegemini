using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class FornecedorDeletadoEvent : DomainEvent
    {
        public FornecedorDeletadoEvent(Guid aggregateId, string nome) : base(aggregateId)
        {
            Id = aggregateId;
            Nome = nome;
        }
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
    }
}
