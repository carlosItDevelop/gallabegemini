// ContaPagaEvent.cs
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ContaPagaEvent : DomainEvent
    {
        public ContaPagaEvent(Guid contaId) : base(contaId)
        {
            ContaId = contaId;
        }

        public Guid ContaId { get; }
    }
}

// Crie também o ContaInativadaEvent, se precisar.