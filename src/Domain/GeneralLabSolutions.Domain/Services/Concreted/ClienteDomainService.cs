using System.Reflection;
using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Validations;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class ClienteDomainService : BaseService, IClienteDomainService
    {

        private readonly IClienteRepository _clienteRepository;
        private readonly IGenericRepository<Cliente, Guid> _query;
        private readonly IMediatorHandler _mediatorHandler;


        #region: Construtor

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="clienteRepository"></param>
        /// <param name="notificador"></param>
        /// <param name="query"></param>
        /// <param name="mediatorHandler"></param>
        public ClienteDomainService(IClienteRepository clienteRepository,
                                    INotificador notificador,
                                    IGenericRepository<Cliente, Guid> query,
                                    IMediatorHandler mediatorHandler)
            : base(notificador)
        {
            _clienteRepository = clienteRepository;
            _query = query;
            this._mediatorHandler = mediatorHandler;
        }

        #endregion


        #region: Regras de Negócios Agrupadas para evitar várias interrupções: Add
        public async Task<bool> ValidarAddCliente(Cliente model)
        {
            bool isValid = true;

            if (_query.SearchAsync(c => c.Documento == model.Documento).Result.Any())
            {
                Notificar("Já existe um Cliente com este documento informado.");
                isValid = false;
            }

            // Cliente novo com Email existente != em Update
            if (_query.SearchAsync(c => c.Email == model.Email).Result.Any())
            {
                Notificar("Já existe um Cliente com este Email. Tente outro!");
                isValid = false;
            }


            if (model.StatusDoCliente == StatusDoCliente.Inativo)
            {
                Notificar("Nenhum cliente pode ser Adicionado com o Status de 'Inativo'.");
                isValid = false;
            }

            if (model.TipoDeCliente == TipoDeCliente.Inadimplente)
            {
                Notificar("Não se pode adicionar um cliente como 'Inadimplente'.");
                isValid = false;
            }

            var clienteExiste = await _query.GetByIdAsync(model.Id);
            if (clienteExiste is not null)
            {
                Notificar("Já existe um Cliente cadastrado com este ID.");
                isValid = false;
            }

            if (!ExecutarValidacao(new ClienteValidation(), model))
            {
                isValid = false;
            }

            return isValid;
        }
        #endregion

        #region: Regras de Negócios: Update
        public async Task<bool> ValidarUpdCliente(Cliente model)
        {
            bool isValid = true;

            // Verificar se o cliente está inadimplente
            // Todo: E se quisermos alterar o TipoDeCliente para 'Comum', assim que ele quitar a dívida?
            // Todo: E se ele está Inadimplente, quer dizer que o seu `StatusDoPedido` deve estar "Inativo" ?
            // Todo: A ideia é; ao alteramos o Tipo do Client para Inadimplente seu Status deve ser alterado para Inativo???
            // Todo: Estas observações acima devem ser levadas em conta ao executar esta validação... !!!?
            var clienteAtual = await _query.GetByIdAsync(model.Id);
            if (clienteAtual != null && clienteAtual.TipoDeCliente == TipoDeCliente.Inadimplente)
            {
                Notificar("Este cliente não pode ser atualizado, pois está inadimplente.");
                isValid = false;
            }


            // Verificar se o email já está em uso por outro cliente
            var clienteComMesmoEmail = await _query.SearchAsync(c => c.Email == model.Email && c.Id != model.Id);
            if (clienteComMesmoEmail.Any())
            {
                Notificar("Já existe um Cliente com este Email. Tente outro!");
                isValid = false;
            }

            // Verificar se o documento já está em uso por outro cliente
            var clienteComMesmoDocumento = await _query.SearchAsync(c => c.Documento == model.Documento && c.Id != model.Id);
            if (clienteComMesmoDocumento.Any())
            {
                Notificar("Já existe um Cliente com este documento informado.");
                isValid = false;
            }


            if (!ExecutarValidacao(new ClienteValidation(), model))
                isValid = false;


            return isValid;
        }
        #endregion

        #region: Regras de Negócios: Delete
        public async Task<bool> ValidarDelCliente(Cliente model)
        {
            bool isValid = true;

            // Todo: E se, ao invés de excluirmos apenas deixá-lo "Inativo"?
            // Todo: Cliente Inativo não pode Comprar... !!?
            var clienteComPedidos = await _query
                .SearchAsync(c => c.Id == model.Id && c.Pedidos.Any());

            if (clienteComPedidos.Any())
            {
                Notificar("O cliente possui pedidos e não pode ser excluído.");
                isValid = false;
            }

            if (!ExecutarValidacao(new DeleteClienteValidation(), model)) isValid = false;

            return isValid;
        }
        #endregion

        #region: Adicionar Cliente

        public async Task AddClienteAsync(Cliente model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarAddCliente(model)) return;


            await _clienteRepository.AddAsync(model);

            model.AdicionarEvento(new ClienteRegistradoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));

            // PersistirDados // Já está sendo feito no AppDbContext

        }

        #endregion

        #region: Atualizar Cliente

        public async Task UpdateClienteAsync(Cliente model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarUpdCliente(model)) return;

            //Não precisa mais disso, pois a entidade já está sendo rastreada
            //e as propriedades já foram atualizadas no controller.
            //await _clienteRepository.UpdateAsync(model);

            model.AdicionarEvento(new ClienteAtualizadoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));

        }

        #endregion

        #region: Deletar Cliente

        public async Task DeleteClienteAsync(Cliente model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarDelCliente(model))
                return;

            // 1. Adiciona o evento
            model.AdicionarEvento(new ClienteDeletadoEvent(model.Id, model.Nome));

            // 2. Publica o evento *ANTES* de qualquer operação de persistência
            await _mediatorHandler.PublicarEvento(new ClienteDeletadoEvent(model.Id, model.Nome));

            // Talvez seja melhor deletar do repositorio e não do genérico
            // e usar Detach, pois não posso instalar o EF aqui; (Exemplo abaixo)
            // // 3. Remove a entidade do DbContext *DIRETAMENTE*
            //_clienteRepository.UnitOfWork.Entry(model).State = EntityState.Deleted;

            await _clienteRepository.DeleteAsync(model); // Substituir esta linha

            // PersistirDados // Já está sendo feito no AppDbContext

        }

        #endregion

        #region: Add, Upd and Delete Dados Bancários

        /// <summary>
        /// Adiciona os dados bancários de um cliente.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <param name="banco"></param>
        /// <param name="agencia"></param>
        /// <param name="conta"></param>
        /// <param name="tipoConta"></param>
        /// <returns></returns>
        public async Task AdicionarDadosBancariosAsync(
    Guid clienteId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2) Apenas altera o domínio
                var novo = cliente.AdicionarDadosBancarios(banco, agencia, conta, tipoConta);

                // 3) Delega à infraestrutura a persistência do filho
                await _clienteRepository.AdicionarDadosBancariosAsync(cliente, novo);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar os dados bancários.");
            }
        }


        public async Task AtualizarDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente == null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                cliente.AtualizarDadosBancarios(dadosBancariosId, banco, agencia, conta, tipoConta);

                // await _clienteRepository.UpdateAsync(cliente); // Provavelmente não necessário se o EF rastreia a entidade filha modificada
                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar o erro completo (ex)
                Notificar("Ocorreu um erro inesperado ao atualizar os dados bancários.");
            }
        }

        public async Task RemoverDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2) Remove apenas no modelo de domínio
                var removido = cliente.RemoverDadosBancarios(dadosBancariosId);

                // 3) Delega à infraestrutura a marcação como Deleted
                await _clienteRepository.RemoverDadosBancariosAsync(cliente, removido);
                // ‑‑ CommitAsync continua sendo chamado pelo Controller ‑‑
            } catch (InvalidOperationException ex)   // Erros lançados pelo domínio
            {
                Notificar(ex.Message);
            } catch (Exception)                      // Qualquer outra exceção inesperada
            {
                Notificar("Ocorreu um erro inesperado ao remover os dados bancários.");
            }
        }


        #endregion

        #region: Métodos Telefone

        /// <summary>
        /// Adiciona um telefone para o cliente especificado.
        /// </summary>
        public async Task AdicionarTelefoneAsync(Guid clienteId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o telefone (e o evento)
                var novoTelefone = cliente.AdicionarTelefone(ddd, numero, tipoTelefone);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _clienteRepository.AdicionarTelefoneAsync(cliente, novoTelefone);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o telefone.");
            }
        }

        /// <summary>
        /// Atualiza um telefone existente do cliente especificado.
        /// </summary>
        public async Task AtualizarTelefoneAsync(Guid clienteId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o telefone (e adicionar evento)
                // A atualização ocorre na entidade Telefone rastreada dentro da coleção de Cliente.
                cliente.AtualizarTelefone(telefoneId, ddd, numero, tipoTelefone);

                // 3. NÃO é necessário chamar _clienteRepository.UpdateAsync(cliente) ou
                //    definir estado explícito para o telefone aqui, pois o EF Core
                //    detecta a modificação na entidade Telefone rastreada.
                //    (Diferente de Add/Remove onde o estado precisa ser explícito)

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o telefone.");
            }
        }

        /// <summary>
        /// Remove um telefone do cliente especificado.
        /// </summary>
        public async Task RemoverTelefoneAsync(Guid clienteId, Guid telefoneId)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var telefoneRemovido = cliente.RemoverTelefone(telefoneId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _clienteRepository.RemoverTelefoneAsync(cliente, telefoneRemovido);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio (ex: telefone não encontrado)
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o telefone.");
            }
        }

        #endregion


        #region Métodos Contatos

        /// <summary>
        /// Adiciona um contato para o cliente especificado.
        /// </summary>
        public async Task AdicionarContatoAsync(
            Guid clienteId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId); // Usar o método específico

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o contato (e o evento)
                var novoContato = cliente.AdicionarContato(
                    nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _clienteRepository.AdicionarContatoAsync(cliente, novoContato);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o contato.");
            }
        }

        /// <summary>
        /// Atualiza um contato existente do cliente especificado.
        /// </summary>
        public async Task AtualizarContatoAsync(
            Guid clienteId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o contato (e adicionar evento)
                // A atualização ocorre na entidade Contato rastreada dentro da coleção de Cliente.
                cliente.AtualizarContato(
                    contatoId, nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. NÃO é necessário chamar _clienteRepository.UpdateAsync(cliente) ou
                //    definir estado explícito para o contato aqui, pois o EF Core
                //    detecta a modificação na entidade Contato rastreada.

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o contato.");
            }
        }

        /// <summary>
        /// Remove um contato do cliente especificado.
        /// </summary>
        public async Task RemoverContatoAsync(Guid clienteId, Guid contatoId)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var contatoRemovido = cliente.RemoverContato(contatoId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _clienteRepository.RemoverContatoAsync(cliente, contatoRemovido);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros (ex: contato não encontrado)
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o contato.");
            }
        }

        #endregion

        #region Métodos Endereços

        /// <summary>
        /// Adiciona um endereço para o cliente especificado.
        /// </summary>
        public async Task AdicionarEnderecoAsync(
            Guid clienteId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId); // Carrega com a coleção de endereços

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                var novoEndereco = cliente.AdicionarEndereco(
                    paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

                await _clienteRepository.AdicionarEnderecoAsync(cliente, novoEndereco);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o endereço.");
            }
        }

        /// <summary>
        /// Atualiza um endereço existente do cliente especificado.
        /// </summary>
        public async Task AtualizarEnderecoAsync(
            Guid clienteId,
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
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                cliente.AtualizarEndereco(
                    enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

                // Não é necessário chamar o repositório para marcar estado de atualização aqui,
                // pois o EF Core rastreia as mudanças na entidade Endereco carregada.
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o endereço.");
            }
        }

        /// <summary>
        /// Remove um endereço do cliente especificado.
        /// </summary>
        public async Task RemoverEnderecoAsync(Guid clienteId, Guid enderecoId)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                var enderecoRemovido = cliente.RemoverEndereco(enderecoId);
                await _clienteRepository.RemoverEnderecoAsync(cliente, enderecoRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o endereço.");
            }
        }

        #endregion


    }
}
