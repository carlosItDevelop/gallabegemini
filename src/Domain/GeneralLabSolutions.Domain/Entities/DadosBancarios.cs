using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class DadosBancarios : EntityBase
    {
        public DadosBancarios(string banco, string agencia, string conta, TipoDeContaBancaria tipoDeContaBancaria, Guid pessoaId)
        {
            Banco = banco;
            Agencia = agencia;
            Conta = conta;
            TipoDeContaBancaria = tipoDeContaBancaria;
            PessoaId = pessoaId;
        }

        // EF
        public DadosBancarios() { }

        public string Banco { get; private set; }
        public string Agencia { get; private set; }
        public string Conta { get; private set; }

        public Guid PessoaId { get; private set; }
        public Pessoa? Pessoa { get; private set; } // Relacionamento com a entidade Pessoa

        public TipoDeContaBancaria TipoDeContaBancaria { get; private set; }

        // Define o banco
        public void SetBanco(string banco) => Banco = banco;
        
        // Define a agência
        public void SetAgencia(string agencia) => Agencia = agencia;
        
        // Define a conta
        public void SetConta(string conta) => Conta = conta;


        public void SetTipoDeContaBancaria(TipoDeContaBancaria tipoDeContaBancaria)
            => TipoDeContaBancaria = tipoDeContaBancaria;

        public void DefinePessoa(Guid pessoaId) => PessoaId = pessoaId;
    }

    
}
