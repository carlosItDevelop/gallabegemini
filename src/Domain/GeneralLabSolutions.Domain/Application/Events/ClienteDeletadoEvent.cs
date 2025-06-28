using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ClienteDeletadoEvent : DomainEvent
    {
        public ClienteDeletadoEvent(Guid aggregateId, string nome) : base(aggregateId)
        {
            Id = aggregateId;
            Nome = nome;
        }

        public Guid Id { get; private set; }

        public string Nome { get; private set; }
    }
}
