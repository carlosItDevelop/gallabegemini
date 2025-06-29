using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Services.Helpers;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Vendedor : EntityAudit, IAggregateRoot, IPessoaContainer
    {
        // EF
        public Vendedor() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="documento"></param>
        /// <param name="tipoDePessoa"></param>
        /// <param name="email"></param>
        public Vendedor(string nome, string documento, TipoDePessoa tipoDePessoa, string email)
        {
            Pessoa = new Pessoa();
            PessoaId = Pessoa.Id;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;

        }

        public Guid PessoaId { get; private set; }
         
        public Pessoa Pessoa { get; private set; }

        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }

        // Email do vendedor
        public string Email { get; set; }
        // Status do vendedor (e.g., Contratado, Freelancer)
        public StatusDoVendedor StatusDoVendedor { get; set; } = StatusDoVendedor.Contratado;
        // Coleção de pedidos realizados pelo vendedor
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();


        public void SetNome(string nome)
            => Nome = nome;

        // Define o tipo de pessoa
        public void SetTipoDePessoa(TipoDePessoa tipoDePessoa)
            => TipoDePessoa = tipoDePessoa;

        // Define o documento da pessoa
        public void SetDocumento(string documento)
            => Documento = documento;



        #region: Adição de dados bancários

        /// <summary>
        /// Adiciona uma nova conta bancária associada.
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
        /// Atualiza os dados de uma conta bancária existente associada a este vendedor.
        /// </summary>
        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            DadosBancariosDomainHelper.AtualizarDadosBancariosGenerico(this, dadosBancariosId, banco, agencia, conta, tipoConta);
        }

        #endregion

        #region: Remoção de dados bancários

        /// <summary>
        /// Remove uma conta bancária associada a este vendedor.
        /// </summary>
        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            return DadosBancariosDomainHelper.RemoverDadosBancariosGenerico(this, dadosBancariosId);
        }


        #endregion

        #region: Adição de Telefone
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            return TelefoneDomainHelper.AdicionarTelefoneGenerico(this, ddd, numero, tipoTelefone);
        }

        #endregion

        #region: Atualização de Telefone

        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            TelefoneDomainHelper.AtualizarTelefoneGenerico(this, telefoneId, ddd, numero, tipoTelefone);
        }

        #endregion

        #region: Remoção de Telefone

        public Telefone RemoverTelefone(Guid telefoneId)
        {
            return TelefoneDomainHelper.RemoverTelefoneGenerico(this, telefoneId);
        }

        #endregion

        #region: Gerenciamento de Contatos

        public Contato AdicionarContato(
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "", // Parâmetros opcionais
            string telefoneAlternativo = "",
            string observacao = "")
        {
            return ContatoDomainHelper.AdicionarContatoGenerico(
                this, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
        }

        /// <summary>
        /// Atualiza os dados de um contato existente associado a este Vendedor.
        /// </summary>
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

        /// <summary>
        /// Remove um contato associado a este Vendedor.
        /// </summary>
        /// <returns>A entidade Contato removida.</returns>
        public Contato RemoverContato(Guid contatoId)
        {
            return ContatoDomainHelper.RemoverContatoGenerico(this, contatoId);
        }

        #endregion

        #region Gerenciamento de Endereços

        /// <summary>
        /// Adiciona um novo endereço associado a este Vendedor.
        /// </summary>
        /// <returns>A entidade Endereco criada.</returns>
        public Endereco AdicionarEndereco(
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco, // Usa o enum interno de Endereco
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            return EnderecoDomainHelper.AdicionarEnderecoGenerico(
                this, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }

        /// <summary>
        /// Atualiza os dados de um endereço existente associado a este Vendedor.
        /// </summary>
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

        /// <summary>
        /// Remove um endereço associado a este Vendedor.
        /// </summary>
        /// <returns>A entidade Endereco removida.</returns>
        public Endereco RemoverEndereco(Guid enderecoId)
        {
            return EnderecoDomainHelper.RemoverEnderecoGenerico(this, enderecoId);
        }

        #endregion


    }
}