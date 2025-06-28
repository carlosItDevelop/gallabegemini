// ContaEstornadaEvent.cs
using GeneralLabSolutions.Domain.Mensageria;


namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ContaEstornadaEvent : DomainEvent
    {
        public ContaEstornadaEvent(Guid contaId) : base(contaId)
        {
            ContaId = contaId;
        }

        public Guid ContaId { get; }
    }
}

// Crie também o ContaInativadaEvent, se precisar.