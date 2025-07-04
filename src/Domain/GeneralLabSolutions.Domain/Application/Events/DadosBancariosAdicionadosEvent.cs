﻿using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class DadosBancariosAdicionadosEvent : DomainEvent
    {
        public Guid DadosBancariosId { get; private set; }
        public string Banco { get; private set; }
        public string Agencia { get; private set; }
        public string Conta { get; private set; }
        public TipoDeContaBancaria TipoDeContaBancaria { get; private set; }

        // AggregateId aqui será o Cliente.Id
        public DadosBancariosAdicionadosEvent(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoDeContaBancaria)
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