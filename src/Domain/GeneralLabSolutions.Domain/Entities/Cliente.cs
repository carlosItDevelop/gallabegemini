using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Cliente : EntityAudit, IAggregateRoot
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

        // Novas propriedades para o cliente

        public string TelefonePrincipal { get; private set; }
        // CRM‑21 Nome contato, crm‑23 Telefone contato
        public string? ContatoRepresentante { get; private set; } = string.Empty; // Nullable string para compatibilidade com o banco de dados

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
            if (Pessoa is null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            var novo = new DadosBancarios(banco, agencia, conta, tipoConta, PessoaId);

            Pessoa.DadosBancarios.Add(novo);

            AdicionarEvento(new DadosBancariosAdicionadosEvent(
                Id, novo.Id, banco, agencia, conta, tipoConta));

            return novo;                     // <-- devolve para a camada de aplicação
        }


        #endregion


        #region: Atualizações de dados bancários

        /// <summary>
        /// Atualiza os dados de uma conta bancária existente associada a este Cliente.
        /// </summary>
        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Encontrar a conta bancária na coleção da Pessoa
            var dadosBancariosParaAtualizar = Pessoa.DadosBancarios.FirstOrDefault(db => db.Id == dadosBancariosId);

            if (dadosBancariosParaAtualizar == null)
            {
                // Ou poderia lançar uma exceção específica ou adicionar um evento de falha
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este cliente.");
                // return; // Alternativa silenciosa
            }

            // Usar os métodos Set da entidade DadosBancarios para atualizar
            dadosBancariosParaAtualizar.SetBanco(banco);
            dadosBancariosParaAtualizar.SetAgencia(agencia);
            dadosBancariosParaAtualizar.SetConta(conta);
            dadosBancariosParaAtualizar.SetTipoDeContaBancaria(tipoConta);

            // Adicionar evento de domínio para notificar sobre a atualização
            AdicionarEvento(new DadosBancariosAtualizadosEvent(
               clienteId: this.Id,
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
        /// Remove uma conta bancária associada a este Cliente.
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


        #region: Gerenciamento de Telefones

        /// <summary>
        /// Adiciona um novo telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone criada.</returns>
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Validações específicas de telefone (ex: formato, unicidade dentro do cliente) poderiam ir aqui
            // if (Pessoa.Telefones.Any(t => t.DDD == ddd && t.Numero == numero))
            // {
            //     throw new InvalidOperationException("Este telefone já está cadastrado para o cliente.");
            // }

            // Cria a nova entidade Telefone usando o PessoaId do Cliente
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

        /// <summary>
        /// Atualiza os dados de um telefone existente associado a este Cliente.
        /// </summary>
        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Encontra o telefone na coleção da Pessoa
            var telefoneParaAtualizar = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaAtualizar == null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este cliente.");
            }

            // Validação de unicidade (se necessário), excluindo o próprio telefone sendo atualizado
            // if (Pessoa.Telefones.Any(t => t.Id != telefoneId && t.DDD == ddd && t.Numero == numero))
            // {
            //     throw new InvalidOperationException("Já existe outro telefone cadastrado com este número para o cliente.");
            // }

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

        /// <summary>
        /// Remove um telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone removida.</returns>
        public Telefone RemoverTelefone(Guid telefoneId)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Encontra o telefone na coleção
            var telefoneParaRemover = Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaRemover == null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este cliente.");
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

        /// <summary>
        /// Adiciona um novo contato associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Contato criada.</returns>
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
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Validações específicas de Contato (ex: não permitir contato com mesmo email principal para este cliente)
            // if (Pessoa.Contatos.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            // {
            //     throw new InvalidOperationException($"Já existe um contato com o email '{email}' para este cliente.");
            // }

            // Cria a nova entidade Contato usando o PessoaId do Cliente
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
        /// Atualiza os dados de um contato existente associado a este Cliente.
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
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Encontra o contato na coleção da Pessoa
            var contatoParaAtualizar = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaAtualizar == null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este cliente.");
            }

            // Validação de unicidade de email (excluindo o próprio contato sendo atualizado)
            // if (Pessoa.Contatos.Any(c => c.Id != contatoId && c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            // {
            //     throw new InvalidOperationException($"Já existe outro contato com o email '{email}' para este cliente.");
            // }

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
        /// Remove um contato associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Contato removida.</returns>
        public Contato RemoverContato(Guid contatoId)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Encontra o contato na coleção
            var contatoParaRemover = Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaRemover == null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este cliente.");
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
        /// Adiciona um novo endereço associado a este Cliente.
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
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            // Validações específicas de Endereço (ex: limitar número de endereços por tipo)
            // if (tipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal && Pessoa.Enderecos.Any(e => e.TipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal))
            // {
            //     throw new InvalidOperationException("Já existe um endereço principal definido para este cliente.");
            // }

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
        /// Atualiza os dados de um endereço existente associado a este Cliente.
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
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            var enderecoParaAtualizar = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaAtualizar == null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este cliente.");
            }

            // Validações (ex: se estiver mudando para Principal, verificar se já existe outro)
            // if (tipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal &&
            //     enderecoParaAtualizar.TipoDeEndereco != Endereco.TipoDeEnderecoEnum.Principal &&
            //     Pessoa.Enderecos.Any(e => e.Id != enderecoId && e.TipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal))
            // {
            //     throw new InvalidOperationException("Já existe outro endereço principal. Não é possível definir este como principal também.");
            // }


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
        /// Remove um endereço associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Endereco removida.</returns>
        public Endereco RemoverEndereco(Guid enderecoId)
        {
            if (Pessoa == null)
                throw new InvalidOperationException("A Pessoa associada ao Cliente não foi carregada.");

            var enderecoParaRemover = Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaRemover == null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este cliente.");
            }

            // Validação (ex: não permitir remover o único endereço principal se for obrigatório ter um)
            // if (enderecoParaRemover.TipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal &&
            //     Pessoa.Enderecos.Count(e => e.TipoDeEndereco == Endereco.TipoDeEnderecoEnum.Principal) <= 1)
            // {
            //     throw new InvalidOperationException("Não é possível remover o único endereço principal.");
            // }

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
