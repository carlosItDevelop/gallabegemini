using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class FornecedorAtualizadoEvent : DomainEvent
    {
        public FornecedorAtualizadoEvent(
            Guid aggregateId,
            string nome,
            string documento,
            TipoDePessoa tipoDePessoa,
            Guid pessoaId,
            StatusDoFornecedor statusDoFornecedor,
            string email) : base(aggregateId)
        {
            Id = aggregateId;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
            PessoaId = pessoaId;
            StatusDoFornecedor = statusDoFornecedor;
            Email = email;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }
        public Guid PessoaId { get; private set; }
        public StatusDoFornecedor StatusDoFornecedor { get; private set; }
        public string Email { get; private set; }

    }
}
