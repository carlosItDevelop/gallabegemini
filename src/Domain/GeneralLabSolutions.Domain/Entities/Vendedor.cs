using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Vendedor : EntityAudit, IAggregateRoot
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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            var novo = new DadosBancarios(banco, agencia, conta, tipoConta, PessoaId);

            Pessoa.DadosBancarios.Add(novo);

            AdicionarEvento(new DadosBancariosAdicionadosEvent(
                Id, novo.Id, banco, agencia, conta, tipoConta));

            return novo;                     // <-- devolve para a camada de aplicação
        }


        #endregion

        #region: Atualizações de dados bancários

        /// <summary>
        /// Atualiza os dados de uma conta bancária existente associada a este vendedor.
        /// </summary>
        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Encontrar a conta bancária na coleção da Pessoa
            var dadosBancariosParaAtualizar = Pessoa.DadosBancarios.FirstOrDefault(db => db.Id == dadosBancariosId);

            if (dadosBancariosParaAtualizar is null)
            {
                // Ou poderia lançar uma exceção específica ou adicionar um evento de falha
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este vendedor.");
                // return; // Alternativa silenciosa
            }

            // Usar os métodos Set da entidade DadosBancarios para atualizar
            dadosBancariosParaAtualizar.SetBanco(banco);
            dadosBancariosParaAtualizar.SetAgencia(agencia);
            dadosBancariosParaAtualizar.SetConta(conta);
            dadosBancariosParaAtualizar.SetTipoDeContaBancaria(tipoConta);

            // Adicionar evento de domínio para notificar sobre a atualização
            AdicionarEvento(new DadosBancariosAtualizadosEvent(
               aggregateId: this.Id,
               dadosBancariosId: dadosBancariosId,
               banco: banco,
               agencia: agencia,
               conta: conta,
               tipoDeContaBancaria: tipoConta
           ));
        }

        #endregion


        #region: Remoção de dados bancários

        /// <summary>
        /// Remove uma conta bancária associada a este vendedor.
        /// </summary>
        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            var db = Pessoa.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId)
                     ?? throw new InvalidOperationException("Dados bancários não encontrados.");

            Pessoa.DadosBancarios.Remove(db);

            AdicionarEvento(new DadosBancariosRemovidosEvent(Id, dadosBancariosId));
            return db;            // devolve o objeto removido
        }


        #endregion

        #region: Adição de Telefone
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Cria a nova entidade Telefone usando o PessoaId do Vendedor
            var novoTelefone = new Telefone(ddd, numero, tipoTelefone, this.PessoaId);

            // Adiciona à coleção na entidade Pessoa
            Pessoa.Telefones.Add(novoTelefone);

            // Adiciona o evento de domínio
            AdicionarEvento(new TelefoneAdicionadoEvent(
                aggregateId: this.Id,
                telefoneId: novoTelefone.Id,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));

            return novoTelefone; // Retorna a entidade criada
        }

        #endregion

        #region: Atualização de Telefone

        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Encontra o telefone na coleção da Pessoa
            var telefoneParaAtualizar = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaAtualizar is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este vendedor.");
            }

            // Usa os métodos Set da entidade Telefone
            telefoneParaAtualizar.SetDDD(ddd);
            telefoneParaAtualizar.SetNumero(numero);
            telefoneParaAtualizar.SetTipoDeTelefone(tipoTelefone);

            // Adiciona o evento de domínio
            AdicionarEvento(new TelefoneAtualizadoEvent(
                aggregateId: this.Id,
                telefoneId: telefoneId,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));
        }

        #endregion

        #region: Remoção de Telefone

        public Telefone RemoverTelefone(Guid telefoneId)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Encontra o telefone na coleção
            var telefoneParaRemover = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaRemover is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este vendedor.");
            }

            // Remove da coleção
            Pessoa.Telefones.Remove(telefoneParaRemover);

            // Adiciona o evento de domínio
            AdicionarEvento(new TelefoneRemovidoEvent(
                aggregateId: this.Id,
                telefoneId: telefoneId
            ));

            return telefoneParaRemover; // Retorna a entidade removida
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
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");


            // Cria a nova entidade Contato usando o PessoaId do Vendedor
            var novoContato = new Contato(
                nome,
                email,
                telefone,
                tipoDeContato,
                this.PessoaId // FK para Pessoa
            )
            {
                // Define propriedades opcionais
                EmailAlternativo = emailAlternativo ?? string.Empty,
                TelefoneAlternativo = telefoneAlternativo ?? string.Empty,
                Observacao = observacao ?? string.Empty
            };

            // Adiciona à coleção na entidade Pessoa
            Pessoa.Contatos.Add(novoContato);

            // Adiciona o evento de domínio
            AdicionarEvento(new ContatoAdicionadoEvent(
                aggregateId: this.Id,
                contatoId: novoContato.Id,
                nome: nome,
                email: email,
                telefone: telefone,
                tipoDeContato: tipoDeContato
            ));

            return novoContato; // Retorna a entidade criada
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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Encontra o contato na coleção da Pessoa
            var contatoParaAtualizar = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este vendedor.");
            }


            // Usa os métodos de atualização da entidade Contato
            contatoParaAtualizar.AtualizarNome(nome);
            contatoParaAtualizar.AtualizarEmail(email);
            contatoParaAtualizar.AtualizarTelefone(telefone);
            contatoParaAtualizar.DefineTipoDeContato(tipoDeContato); // Usa o método Set
            contatoParaAtualizar.AtualizarEmailAlternativo(emailAlternativo);
            contatoParaAtualizar.AtualizarTelefoneAlternativo(telefoneAlternativo);
            contatoParaAtualizar.AtualizarObservacao(observacao);


            // Adiciona o evento de domínio
            AdicionarEvento(new ContatoAtualizadoEvent(
                aggregateId: this.Id,
                contatoId: contatoId,
                nome: nome,
                email: email,
                telefone: telefone,
                tipoDeContato: tipoDeContato
            ));
        }

        /// <summary>
        /// Remove um contato associado a este Vendedor.
        /// </summary>
        /// <returns>A entidade Contato removida.</returns>
        public Contato RemoverContato(Guid contatoId)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            // Encontra o contato na coleção
            var contatoParaRemover = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaRemover is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este vendedor.");
            }

            // Remove da coleção
            Pessoa.Contatos.Remove(contatoParaRemover);

            // Adiciona o evento de domínio
            AdicionarEvento(new ContatoRemovidoEvent(
                aggregateId: this.Id,
                contatoId: contatoId
            ));

            return contatoParaRemover; // Retorna a entidade removida
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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            var novoEndereco = new Endereco(
                this.PessoaId, // FK para Pessoa
                paisCodigoIso,
                linhaEndereco1,
                cidade,
                codigoPostal,
                tipoDeEndereco,
                linhaEndereco2,
                estadoOuProvincia,
                informacoesAdicionais
            );

            Pessoa.Enderecos.Add(novoEndereco);

            AdicionarEvento(new EnderecoAdicionadoEvent(
                aggregateId: this.Id,
                enderecoId: novoEndereco.Id,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));

            return novoEndereco;
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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            var enderecoParaAtualizar = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este vendedor.");
            }


            enderecoParaAtualizar.SetPaisCodigoIso(paisCodigoIso);
            enderecoParaAtualizar.SetLinhaEndereco1(linhaEndereco1);
            enderecoParaAtualizar.SetLinhaEndereco2(linhaEndereco2);
            enderecoParaAtualizar.SetCidade(cidade);
            enderecoParaAtualizar.SetEstadoOuProvincia(estadoOuProvincia);
            enderecoParaAtualizar.SetCodigoPostal(codigoPostal);
            enderecoParaAtualizar.SetTipoDeEndereco(tipoDeEndereco);
            enderecoParaAtualizar.SetInformacoesAdicionais(informacoesAdicionais);

            AdicionarEvento(new EnderecoAtualizadoEvent(
                aggregateId: this.Id,
                enderecoId: enderecoId,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));
        }

        /// <summary>
        /// Remove um endereço associado a este Vendedor.
        /// </summary>
        /// <returns>A entidade Endereco removida.</returns>
        public Endereco RemoverEndereco(Guid enderecoId)
        {
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Vendedor não foi carregada.");

            var enderecoParaRemover = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaRemover == null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este vendedor.");
            }


            Pessoa.Enderecos.Remove(enderecoParaRemover);

            AdicionarEvento(new EnderecoRemovidoEvent(
                aggregateId: this.Id,
                enderecoId: enderecoId
            ));

            return enderecoParaRemover;
        }

        #endregion


    }
}