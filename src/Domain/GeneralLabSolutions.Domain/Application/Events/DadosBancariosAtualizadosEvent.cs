using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class DadosBancariosAtualizadosEvent : DomainEvent
    {
        public Guid DadosBancariosId { get; private set; }
        // Poderia incluir os valores antigos/novos se necessário para o handler
        public string Banco { get; private set; }
        public string Agencia { get; private set; }
        public string Conta { get; private set; }
        public TipoDeContaBancaria TipoDeContaBancaria { get; private set; }


        // AggregateId aqui será o Cliente.Id
        public DadosBancariosAtualizadosEvent(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoDeContaBancaria)
            : base(clienteId)
        {
            DadosBancariosId = dadosBancariosId;
            Banco = banco;
            Agencia = agencia;
            Conta = conta;
            TipoDeContaBancaria = tipoDeContaBancaria;
        }
    }
}