using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Services.Helpers;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Cliente : EntityAudit, IAggregateRoot, IPessoaContainer
    {
        // EF
        public Cliente() { }

        #region: Contrutor Parametrizado

        /// <summary>
        /// Construtor para criar um cliente com os dados necessários.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="documento"></param>
        /// <param name="tipoDePessoa"></param>
        /// <param name="email"></param>
        public Cliente(string nome, string documento, TipoDePessoa tipoDePessoa, string email)
        {
            Pessoa = new Pessoa();
            PessoaId = Pessoa.Id;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
        }

        #endregion


        #region: Propriedades

        public Guid PessoaId { get; set; }

        public Pessoa Pessoa { get; set; }

        public string Nome { get; private set; }

        public string Documento { get; private set; }

        public string Email { get; private set; }

        public string? InscricaoEstadual { get; set; } = string.Empty;

        public string? Observacao { get; set; } = string.Empty;


        public string TelefonePrincipal { get; private set; }
        // CRM‑21 Nome contato, crm‑23 Telefone contato
        public string? ContatoRepresentante { get; private set; } = string.Empty;

        public void SetContatoRepresentante(string contatoRepresentante)
            => ContatoRepresentante = contatoRepresentante;

        public void SetTelefonePrincipal(string telefonePrincipal)
            => TelefonePrincipal = telefonePrincipal;




        #endregion


        #region: Enums e Collections

        public TipoDePessoa TipoDePessoa { get; private set; }

        // Status do cliente (e.g., Ativo, Inativo)
        public StatusDoCliente StatusDoCliente { get; set; } = StatusDoCliente.Ativo;
        // Tipo de cliente (e.g., Comum, Especial)
        public TipoDeCliente TipoDeCliente { get; set; }
            = TipoDeCliente.Comum;
        // Coleção de pedidos realizados pelo cliente
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        #endregion


        #region: Métodos Ad Hoc

        // Define o email do cliente
        public void SetEmail(string newEmail) => Email = newEmail;

        public void AddPedido(Pedido pedido)
            => Pedidos.Add(pedido);

        public void SetNome(string nome) => Nome = nome;

        // Define o tipo de pessoa
        public void SetTipoDePessoa(TipoDePessoa tipoDePessoa)
            => TipoDePessoa = tipoDePessoa;

        // Define o documento da pessoa
        public void SetDocumento(string documento)
            => Documento = documento;

        #endregion


        #region: Adição de dados bancários

        /// <summary>
        /// Adiciona uma nova conta bancária associada a este Cliente.
        /// </summary>
        public DadosBancarios AdicionarDadosBancarios(string banco,
                string agencia,
                string conta,
                TipoDeContaBancaria tipoConta)
        {
            return DadosBancariosDomainHelper.AdicionarDadosBancariosGenerico(this, banco, agencia, conta, tipoConta);
        }


        #endregion


        #region: Atualizações de dados bancários

        /// <summary>
        /// Atualiza os dados de uma conta bancária existente associada a este Cliente.
        /// </summary>
        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            DadosBancariosDomainHelper.AtualizarDadosBancariosGenerico(this, dadosBancariosId, banco, agencia, conta, tipoConta);
        }

        #endregion


        #region: Remoção de dados bancários

        /// <summary>
        /// Remove uma conta bancária associada a este Cliente.
        /// </summary>
        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            return DadosBancariosDomainHelper.RemoverDadosBancariosGenerico(this, dadosBancariosId);
        }


        #endregion


        #region: Gerenciamento de Telefones

        /// <summary>
        /// Adiciona um novo telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone criada.</returns>
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            return TelefoneDomainHelper.AdicionarTelefoneGenerico(this, ddd, numero, tipoTelefone);
        }

        /// <summary>
        /// Atualiza os dados de um telefone existente associado a este Cliente.
        /// </summary>
        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            TelefoneDomainHelper.AtualizarTelefoneGenerico(this, telefoneId, ddd, numero, tipoTelefone);
        }

        /// <summary>
        /// Remove um telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone removida.</returns>
        public Telefone RemoverTelefone(Guid telefoneId)
        {
            return TelefoneDomainHelper.RemoverTelefoneGenerico(this, telefoneId);
        }

        #endregion


        #region: Gerenciamento de Contatos

        /// <summary>
        /// Adiciona um novo contato associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Contato criada.</returns>
        public Contato AdicionarContato(
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            return ContatoDomainHelper.AdicionarContatoGenerico(
                this, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
        }

        public void AtualizarContato(
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            ContatoDomainHelper.AtualizarContatoGenerico(
                this, contatoId, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
        }

        public Contato RemoverContato(Guid contatoId)
        {
            return ContatoDomainHelper.RemoverContatoGenerico(this, contatoId);
        }

        #endregion


        #region Gerenciamento de Endereços

        /// <summary>
        /// Adiciona um novo endereço associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Endereco criada.</returns>
        public Endereco AdicionarEndereco(
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            return EnderecoDomainHelper.AdicionarEnderecoGenerico(
                this, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }

        public void AtualizarEndereco(
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            EnderecoDomainHelper.AtualizarEnderecoGenerico(
                this, enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }

        public Endereco RemoverEndereco(Guid enderecoId)
        {
            return EnderecoDomainHelper.RemoverEnderecoGenerico(this, enderecoId);
        }

        #endregion


    }
}
