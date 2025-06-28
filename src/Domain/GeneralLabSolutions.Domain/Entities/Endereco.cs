using GeneralLabSolutions.Domain.Entities.Base;
using System; // Para Guid

namespace GeneralLabSolutions.Domain.Entities
{
    public class Endereco : EntityBase
    {
        // Enum TipoDeEndereco movido para dentro da classe
        public enum TipoDeEnderecoEnum // Renomeado para evitar conflito se houver um global
        {
            Principal = 1,
            Cobranca = 2,
            Entrega = 3,
            Outro = 4
        }

        // EF Core Constructor
        public Endereco() { }

        public Endereco(
            Guid pessoaId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            TipoDeEnderecoEnum tipoDeEndereco, // Usa o enum interno
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            PessoaId = pessoaId;
            PaisCodigoIso = paisCodigoIso;
            LinhaEndereco1 = linhaEndereco1;
            Cidade = cidade;
            CodigoPostal = codigoPostal;
            TipoDeEndereco = tipoDeEndereco;
            LinhaEndereco2 = linhaEndereco2 ?? string.Empty;
            EstadoOuProvincia = estadoOuProvincia ?? string.Empty;
            InformacoesAdicionais = informacoesAdicionais ?? string.Empty;
        }

        public Guid PessoaId { get; private set; }
        public virtual Pessoa Pessoa { get; private set; }

        public string PaisCodigoIso { get; private set; }
        public string LinhaEndereco1 { get; private set; }
        public string LinhaEndereco2 { get; private set; }
        public string Cidade { get; private set; }
        public string EstadoOuProvincia { get; private set; }
        public string CodigoPostal { get; private set; }
        public string InformacoesAdicionais { get; private set; }
        public TipoDeEnderecoEnum TipoDeEndereco { get; private set; } // Usa o enum interno

        #region Métodos Set
        public void DefinePessoa(Guid pessoaId) => PessoaId = pessoaId;
        public void SetPaisCodigoIso(string pais) => PaisCodigoIso = pais;
        public void SetLinhaEndereco1(string linha1) => LinhaEndereco1 = linha1;
        public void SetLinhaEndereco2(string? linha2) => LinhaEndereco2 = linha2 ?? string.Empty;
        public void SetCidade(string cidade) => Cidade = cidade;
        public void SetEstadoOuProvincia(string? estado) => EstadoOuProvincia = estado ?? string.Empty;
        public void SetCodigoPostal(string cep) => CodigoPostal = cep;
        public void SetInformacoesAdicionais(string? info) => InformacoesAdicionais = info ?? string.Empty;
        public void SetTipoDeEndereco(TipoDeEnderecoEnum tipo) => TipoDeEndereco = tipo;
        #endregion
    }
}