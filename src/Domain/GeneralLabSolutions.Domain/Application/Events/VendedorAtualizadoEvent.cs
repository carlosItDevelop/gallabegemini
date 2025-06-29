using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class VendedorAtualizadoEvent : DomainEvent
    {
        public VendedorAtualizadoEvent(Guid aggregateId, string nome, string documento, TipoDePessoa tipoDePessoa, string email) : base(aggregateId)
        {
            Id = aggregateId;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
        }

        public Guid Id { get; private set; }

        public string Nome { get; private set; }

        public string Documento { get; private set; }
        public string Email { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }
    }
}


