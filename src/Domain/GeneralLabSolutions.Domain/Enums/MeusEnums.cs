using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GeneralLabSolutions.Domain.Enums
{

    public enum TipoDeConta
    {
        [Description("Conta a Receber")] Receber = 1,
        [Description("Conta a Pagar")] Pagar = 2
    }

    public enum Sexo
    {
        [Description("Feminino")] Feminino, // Peso: 48
        [Description("Masculino")] Masculino, // Peso: 48
        [Description("Outro")] Outro // Peso: 4

    }
    public enum StatusDoCliente
    {
        [Description("Ativo")] Ativo, // Peso: 30
        [Description("Inativo")] Inativo, // Peso 8
        [Description("Bloqueado")] Bloqueado // Peso 3  // // usado pelo FIN‑16 (bloqueio de faturamento)
    }

    public enum RegimeTributario   // CRM‑14 campos fiscais
    {
        [Description("Simples Nacional")] SimplesNacional = 1, // Peso: 8
        [Description("Lucro Presumido")] LucroPresumido = 2,  // Peso: 12
        [Description("Lucro Real")] LucroReal = 3,       // Peso: 20
        [Description("Outro")] Outro = 99           // Peso: 2
    }

    public enum StatusDoFornecedor
    {
        Ativo, // Peso: 30
        Inativo // Peso: 5
    }


    public enum StatusDoProduto
    {
        [Description("Dropshipping")] Dropshipping, // Peso: 30 => Trocado por Dropshipping
        [Description("Inativo")] Inativo, // Peso: 5
        [Description("Reservado")] Reservado, // 20
        [Description("Em Estoque")] EmEstoque, // 10
        [Description("Esgotado")] Esgotado // Todo: Peso 5
    }
    public enum StatusDoVendedor
    {
        Admin, // Peso: 15
        Contratado, // Peso: 10
        FreeLance, // Peso: 20
        Inativo // Peso: 5
    }
    public enum TipoDeCliente
    {
        [Description("Especial")] Especial, // Peso 9
        [Description("Comum")] Comum, // Peso 32
        [Description("Inadimplente")] Inadimplente // Peso 4
    }
    public enum TipoDeContato
    {
        Inativo, // Peso: 5
        Comercial, // Peso: 35
        Pessoal, // Peso: 10
        ProspeccaoCliente, // Peso: 25
        ProspeccaoVendedor, // Peso: 10
        ProspeccaoFornecedor // Peso: 15
    }
    public enum TipoDePessoa
    {
        [Description("Física")] Fisica, // Peso: 5
        [Description("Jurídica")] Juridica // Peso: 25
    }
    public enum TipoDeTelefone
    {
        Celular, // Peso: 10
        Residencial, // Peso: 5
        Comercial, // Peso: 15
        Recado, // Peso: 5
        Outro // Peso: 3
    }

    // TipoDescontoVoucher
    public enum TipoDescontoVoucher
    {
        Porcentagem = 0,
        Valor = 1
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Column
    {
        [EnumMember(Value = "todo")]
        todo,

        [EnumMember(Value = "review")]
        review,

        [EnumMember(Value = "progress")]
        progress,

        [EnumMember(Value = "done")]
        done
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Priority
    {
        [EnumMember(Value = "alta")]
        alta,

        [EnumMember(Value = "media")]
        media,

        [EnumMember(Value = "baixa")]
        baixa
    }


    public enum TipoEvento
    {
        [Display(Name = "bg-danger-subtle")] Perigo,
        [Display(Name = "bg-success-subtle")] Sucesso,
        [Display(Name = "bg-primary-subtle")] Primario,
        [Display(Name = "bg-info-subtle")] Info,
        [Display(Name = "bg-dark-subtle")] Escuro,
        [Display(Name = "bg-warning-subtle")] Alerta
    }

    // Enums para CRM Leads


    public enum LeadStatus
    {
        [EnumMember(Value = "novo")]
        Novo,
        [EnumMember(Value = "contato")]
        Contato,
        [EnumMember(Value = "qualificado")]
        Qualificado,
        [EnumMember(Value = "proposta")]
        Proposta,
        [EnumMember(Value = "negociacao")]
        Negociacao,
        [EnumMember(Value = "ganho")]
        Ganho,
        [EnumMember(Value = "perdido")]
        Perdido
    }

    public enum LeadTemperature
    {
        [EnumMember(Value = "quente")]
        Quente,
        [EnumMember(Value = "morno")]
        Morno,
        [EnumMember(Value = "frio")]
        Frio
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CrmTaskStatus
    {
        [EnumMember(Value = "pending")]
        Pendente,

        [EnumMember(Value = "completed")]
        Concluida,

        [EnumMember(Value = "cancelled")]
        Cancelada
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {
        [EnumMember(Value = "high")]
        Alta,

        [EnumMember(Value = "medium")]
        Media,

        [EnumMember(Value = "low")]
        Baixa
    }

    public enum TipoDeContaBancaria
    {
        Corrente = 1,
        Poupanca = 2,
        ContaSalario = 3,
        ContaConjunta = 4,
        ContaUniversitaria = 5,
        ContaEmpresarial = 6,
        Pagamento = 7 // contas digitais
    }
}
