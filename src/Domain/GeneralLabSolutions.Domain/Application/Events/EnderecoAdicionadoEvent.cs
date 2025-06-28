using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Entities; // Para Endereco.TipoDeEnderecoEnum
using System;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class EnderecoAdicionadoEvent : DomainEvent
    {
        public Guid EnderecoId { get; private set; }
        public string PaisCodigoIso { get; private set; }
        public string LinhaEndereco1 { get; private set; }
        public string Cidade { get; private set; }
        public string CodigoPostal { get; private set; }
        public Endereco.TipoDeEnderecoEnum TipoDeEndereco { get; private set; }
        // Adicione outras propriedades se forem relevantes para os handlers

        // AggregateId será o ID do Cliente/Fornecedor/Vendedor
        public EnderecoAdicionadoEvent(
            Guid aggregateId,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco)
            : base(aggregateId)
        {
            EnderecoId = enderecoId;
            PaisCodigoIso = paisCodigoIso;
            LinhaEndereco1 = linhaEndereco1;
            Cidade = cidade;
            CodigoPostal = codigoPostal;
            TipoDeEndereco = tipoDeEndereco;
        }
    }
}