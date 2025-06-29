using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class VendedorDeletadoEvent : DomainEvent
    {
        public VendedorDeletadoEvent(Guid aggregateId) : base(aggregateId)
        {
            Id = aggregateId;
        }

        public Guid Id { get; private set; }

    }
}


