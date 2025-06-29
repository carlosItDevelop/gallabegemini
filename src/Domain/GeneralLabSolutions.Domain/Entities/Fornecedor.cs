// Local do Arquivo: GeneralLabSolutions.Domain/Entities/Fornecedor.cs

using GeneralLabSolutions.Domain.Application.Events; // Precisaremos criar estes eventos
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Services.Helpers;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Fornecedor : EntityAudit, IAggregateRoot, IPessoaContainer
    {
        // EF Core constructor
        public Fornecedor() { }

        // Construtor principal
        public Fornecedor(
            string nome,
            string documento,
            TipoDePessoa tipoDePessoa,
            string email)
        {
            Pessoa = new Pessoa();
            PessoaId = Pessoa.Id;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
        }

        #region Propriedades Principais
        public Guid PessoaId { get; private set; }
        public Pessoa Pessoa { get; private set; }

        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }
        public string Email { get; private set; }
        public StatusDoFornecedor StatusDoFornecedor { get; set; } = StatusDoFornecedor.Ativo;

        // Relação 1:N com Produtos
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
        #endregion

        #region Métodos de Setter
        public void SetNome(string nome) => Nome = nome;
        public void SetTipoDePessoa(TipoDePessoa tipoDePessoa) => TipoDePessoa = tipoDePessoa;
        public void SetDocumento(string documento) => Documento = documento;
        public void SetEmail(string email) => Email = email;
        public void AddProduto(Produto produto) => Produtos.Add(produto);
        #endregion

        // ==============================================================================
        // INÍCIO DA IMPLEMENTAÇÃO DA RAIZ DE AGREGAÇÃO (Análogo ao Cliente.cs)
        // ==============================================================================

        #region Gerenciamento de Dados Bancários
        public DadosBancarios AdicionarDadosBancarios(string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            return DadosBancariosDomainHelper.AdicionarDadosBancariosGenerico(this, banco, agencia, conta, tipoConta);
        }

        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            DadosBancariosDomainHelper.AtualizarDadosBancariosGenerico(this, dadosBancariosId, banco, agencia, conta, tipoConta);
        }

        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            return DadosBancariosDomainHelper.RemoverDadosBancariosGenerico(this, dadosBancariosId);
        }
        #endregion

        #region Gerenciamento de Telefones
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            return TelefoneDomainHelper.AdicionarTelefoneGenerico(this, ddd, numero, tipoTelefone);
        }

        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            TelefoneDomainHelper.AtualizarTelefoneGenerico(this, telefoneId, ddd, numero, tipoTelefone);
        }

        public Telefone RemoverTelefone(Guid telefoneId)
        {
            return TelefoneDomainHelper.RemoverTelefoneGenerico(this, telefoneId);
        }
        #endregion

        #region Gerenciamento de Contatos (Método de Atualização CORRIGIDO)
        public Contato AdicionarContato(string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            return ContatoDomainHelper.AdicionarContatoGenerico(this, nome, email, telefone, tipoDeContato, emailAlt, telAlt, obs);
        }

        
        public void AtualizarContato(Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            ContatoDomainHelper.AtualizarContatoGenerico(this, contatoId, nome, email, telefone, tipoDeContato, emailAlt, telAlt, obs);
        }

        public Contato RemoverContato(Guid contatoId)
        {
            return ContatoDomainHelper.RemoverContatoGenerico(this, contatoId);
        }
        #endregion

        #region Gerenciamento de Endereços (Método de Atualização CORRIGIDO)
        public Endereco AdicionarEndereco(string pais, string linha1, string cidade, string cep, Endereco.TipoDeEnderecoEnum tipo, string? linha2, string? estado, string? info)
        {
            return EnderecoDomainHelper.AdicionarEnderecoGenerico(this, pais, linha1, cidade, cep, tipo, linha2, estado, info);
        }
        
        public void AtualizarEndereco(Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            EnderecoDomainHelper.AtualizarEnderecoGenerico(this, enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco, linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }
        

        public Endereco RemoverEndereco(Guid enderecoId)
        {
            return EnderecoDomainHelper.RemoverEnderecoGenerico(this, enderecoId);
        }
        #endregion
    }
}