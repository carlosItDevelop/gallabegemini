// Local do Arquivo: GeneralLabSolutions.Domain/Entities/Fornecedor.cs

using GeneralLabSolutions.Domain.Application.Events; // Precisaremos criar estes eventos
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Fornecedor : EntityAudit, IAggregateRoot // MUDANÇA 1: Herda de EntityAudit e é uma IAggregateRoot
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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Fornecedor não foi carregada.");

            var novoDb = new DadosBancarios(banco, agencia, conta, tipoConta, PessoaId);
            Pessoa.DadosBancarios.Add(novoDb);

            // Dispara evento de domínio
            AdicionarEvento(new DadosBancariosAdicionadosEvent(this.Id, novoDb.Id, banco, agencia, conta, tipoConta));
            return novoDb;
        }

        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            if (Pessoa?.DadosBancarios is null)
                throw new InvalidOperationException("A coleção de Dados Bancários não foi carregada.");

            var dbParaAtualizar = Pessoa.DadosBancarios.FirstOrDefault(db => db.Id == dadosBancariosId)
                ?? throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados.");

            dbParaAtualizar.SetBanco(banco);
            dbParaAtualizar.SetAgencia(agencia);
            dbParaAtualizar.SetConta(conta);
            dbParaAtualizar.SetTipoDeContaBancaria(tipoConta);

            AdicionarEvento(new DadosBancariosAtualizadosEvent(this.Id, dadosBancariosId, banco, agencia, conta, tipoConta));
        }

        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            if (Pessoa?.DadosBancarios is null)
                throw new InvalidOperationException("A coleção de Dados Bancários não foi carregada.");

            var db = Pessoa.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId)
                ?? throw new InvalidOperationException("Dados bancários não encontrados.");

            Pessoa.DadosBancarios.Remove(db);
            AdicionarEvento(new DadosBancariosRemovidosEvent(this.Id, dadosBancariosId));
            return db;
        }
        #endregion

        #region Gerenciamento de Telefones
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("Pessoa não carregada.");
            var novoTelefone = new Telefone(ddd, numero, tipoTelefone, this.PessoaId);
            Pessoa.Telefones.Add(novoTelefone);
            AdicionarEvento(new TelefoneAdicionadoEvent(this.Id, novoTelefone.Id, ddd, numero, tipoTelefone));
            return novoTelefone;
        }

        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa?.Telefones is null)
                throw new InvalidOperationException("Coleção de Telefones não carregada.");
            var telefone = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId) ?? throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado.");
            telefone.SetDDD(ddd);
            telefone.SetNumero(numero);
            telefone.SetTipoDeTelefone(tipoTelefone);
            AdicionarEvento(new TelefoneAtualizadoEvent(this.Id, telefoneId, ddd, numero, tipoTelefone));
        }

        public Telefone RemoverTelefone(Guid telefoneId)
        {
            if (Pessoa?.Telefones is null)
                throw new InvalidOperationException("Coleção de Telefones não carregada.");
            var telefone = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId) ?? throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado.");
            Pessoa.Telefones.Remove(telefone);
            AdicionarEvento(new TelefoneRemovidoEvent(this.Id, telefoneId));
            return telefone;
        }
        #endregion

        #region Gerenciamento de Contatos (Método de Atualização CORRIGIDO)
        public Contato AdicionarContato(string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            if (Pessoa is null)
                throw new InvalidOperationException("Pessoa não carregada.");
            var novoContato = new Contato(nome, email, telefone, tipoDeContato, this.PessoaId)
            {
                EmailAlternativo = emailAlt,
                TelefoneAlternativo = telAlt,
                Observacao = obs
            };
            Pessoa.Contatos.Add(novoContato);
            AdicionarEvento(new ContatoAdicionadoEvent(this.Id, novoContato.Id, nome, email, telefone, tipoDeContato));
            return novoContato;
        }

        
        public void AtualizarContato(Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            if (Pessoa?.Contatos is null)
                throw new InvalidOperationException("A coleção de Contatos do Fornecedor não foi carregada.");

            var contatoParaAtualizar = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId)
                ?? throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este fornecedor.");

            // Chamando os Setters individuais (ou um método de atualização na entidade Contato, se existir)
            contatoParaAtualizar.AtualizarNome(nome);
            contatoParaAtualizar.AtualizarEmail(email);
            contatoParaAtualizar.AtualizarTelefone(telefone);
            contatoParaAtualizar.DefineTipoDeContato(tipoDeContato);
            contatoParaAtualizar.AtualizarEmailAlternativo(emailAlt);
            contatoParaAtualizar.AtualizarTelefoneAlternativo(telAlt);
            contatoParaAtualizar.AtualizarObservacao(obs);

            AdicionarEvento(new ContatoAtualizadoEvent(this.Id, contatoId, nome, email, telefone, tipoDeContato));
        }

        public Contato RemoverContato(Guid contatoId)
        {
            if (Pessoa?.Contatos is null)
                throw new InvalidOperationException("Coleção de Contatos não carregada.");
            var contato = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId) ?? throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado.");
            Pessoa.Contatos.Remove(contato);
            AdicionarEvento(new ContatoRemovidoEvent(this.Id, contatoId));
            return contato;
        }
        #endregion

        #region Gerenciamento de Endereços (Método de Atualização CORRIGIDO)
        public Endereco AdicionarEndereco(string pais, string linha1, string cidade, string cep, Endereco.TipoDeEnderecoEnum tipo, string? linha2, string? estado, string? info)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("Pessoa não carregada.");
            var novoEndereco = new Endereco(this.PessoaId, pais, linha1, cidade, cep, tipo, linha2, estado, info);
            Pessoa.Enderecos.Add(novoEndereco);
            AdicionarEvento(new EnderecoAdicionadoEvent(this.Id, novoEndereco.Id, pais, linha1, cidade, cep, tipo));
            return novoEndereco;
        }
        
        public void AtualizarEndereco(Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            if (Pessoa?.Enderecos is null)
                throw new InvalidOperationException("A coleção de Endereços do Fornecedor não foi carregada.");

            var enderecoParaAtualizar = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId)
                ?? throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este fornecedor.");

            // Chamando os Setters individuais na entidade Endereço
            enderecoParaAtualizar.SetPaisCodigoIso(paisCodigoIso);
            enderecoParaAtualizar.SetLinhaEndereco1(linhaEndereco1);
            enderecoParaAtualizar.SetLinhaEndereco2(linhaEndereco2);
            enderecoParaAtualizar.SetCidade(cidade);
            enderecoParaAtualizar.SetEstadoOuProvincia(estadoOuProvincia);
            enderecoParaAtualizar.SetCodigoPostal(codigoPostal);
            enderecoParaAtualizar.SetTipoDeEndereco(tipoDeEndereco);
            enderecoParaAtualizar.SetInformacoesAdicionais(informacoesAdicionais);

            AdicionarEvento(new EnderecoAtualizadoEvent(this.Id, enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco));
        }
        

        public Endereco RemoverEndereco(Guid enderecoId)
        {
            if (Pessoa?.Enderecos is null)
                throw new InvalidOperationException("Coleção de Endereços não carregada.");
            var endereco = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId) ?? throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado.");
            Pessoa.Enderecos.Remove(endereco);
            AdicionarEvento(new EnderecoRemovidoEvent(this.Id, enderecoId));
            return endereco;
        }
        #endregion
    }
}