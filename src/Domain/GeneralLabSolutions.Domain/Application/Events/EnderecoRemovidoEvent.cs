using GeneralLabSolutions.Domain.Mensageria;
using System;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class EnderecoRemovidoEvent : DomainEvent
    {
        public Guid EnderecoId { get; private set; }

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor
        public EnderecoRemovidoEvent(Guid aggregateId, Guid enderecoId)
            : base(aggregateId)
        {
            EnderecoId = enderecoId;
        }
    }
}