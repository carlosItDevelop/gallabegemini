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
    public class VendedorDomainService : BaseService, IVendedorDomainService
    {
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IGenericRepository<Vendedor, Guid> _query;
        private readonly IMediatorHandler _mediatorHandler;

        public VendedorDomainService(
            IVendedorRepository vendedorRepository, 
            IGenericRepository<Vendedor, Guid> query,
            INotificador notificador,
            IMediatorHandler mediatorHandler) : base(notificador)
        {
            _vendedorRepository = vendedorRepository;
            _query = query;
            this._mediatorHandler = mediatorHandler;
        }


        #region: Regras de Negócios
        public async Task AddVendedorAsync(Vendedor model) // Nome padronizado
        {
            if (!await ValidarSalvarVendedor(model, isUpdate: false))
                return;
            await _vendedorRepository.AddAsync(model);

            model.AdicionarEvento(new VendedorRegistradoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));
        }

        public async Task UpdateVendedorAsync(Vendedor model) // Nome padronizado
        {
            if (!await ValidarSalvarVendedor(model, isUpdate: true)) return;

            model.AdicionarEvento(new VendedorAtualizadoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));
        }

        public async Task DeleteVendedorAsync(Vendedor model)
        {
            if (!await ValidarRemocaoVendedor(model)) return;

            await _vendedorRepository.DeleteAsync(model);

            // Criar o Evento de exclusão
            // PersistirDados // Já está sendo feito no AppDbContext
        }

        #endregion

        public async Task<Vendedor?> ObterVendedorPorId(Guid id)
        {
            return await _vendedorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Vendedor>> ObterTodosVendedores()
        {
            return await _vendedorRepository.GetAllAsync();
        }

        #region: DADOS BANCÁRIOS

        public async Task AdicionarDadosBancariosAsync(Guid vendedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);

            if (vendedor is null) 
            { 
                Notificar($"Vendedor com ID {vendedorId} não encontrado."); return; 
            }

            try
            {
                // 2) Apenas altera o domínio
                var novo = vendedor.AdicionarDadosBancarios(banco, agencia, conta, tipoConta);

                // 3) Delega à infraestrutura a persistência do filho
                await _vendedorRepository.AdicionarDadosBancariosAsync(vendedor, novo);

            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar os dados bancários.");
            }


        }



        public async Task AtualizarDadosBancariosAsync(Guid vendedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var vendedor = await _vendedorRepository.ObterVendedorComDadosBancarios(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                vendedor.AtualizarDadosBancarios(dadosBancariosId, banco, agencia, conta, tipoConta);

                // await _vendedorRepository.UpdateAsync(vendedor); // Provavelmente não necessário se o EF rastreia a entidade filha modificada
                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                // Logar o erro completo (ex)
                Notificar("Ocorreu um erro inesperado ao atualizar os dados bancários.");
            }
        }

        public async Task RemoverDadosBancariosAsync(Guid vendedorId, Guid dadosBancariosId)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var vendedor = await _vendedorRepository.ObterVendedorComDadosBancarios(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2) Remove apenas no modelo de domínio
                var removido = vendedor.RemoverDadosBancarios(dadosBancariosId);

                // 3) Delega à infraestrutura a marcação como Deleted
                await _vendedorRepository.RemoverDadosBancariosAsync(vendedor, removido);
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


        #region: TELEFONES

        public async Task AdicionarTelefoneAsync(Guid vendedorId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Telefones
            var vendedor = await _vendedorRepository.ObterVendedorComTelefones(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o telefone (e o evento)
                var novoTelefone = vendedor.AdicionarTelefone(ddd, numero, tipoTelefone);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _vendedorRepository.AdicionarTelefoneAsync(vendedor, novoTelefone);

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


        public async Task AtualizarTelefoneAsync(Guid vendedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Telefones
            var vendedor = await _vendedorRepository.ObterVendedorComTelefones(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o telefone (e adicionar evento)
                // A atualização ocorre na entidade Telefone rastreada dentro da coleção de Vendedor.
                vendedor.AtualizarTelefone(telefoneId, ddd, numero, tipoTelefone);

                // 3. NÃO é necessário chamar _vendedorRepository.UpdateAsync(vendedor) ou
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



        public async Task RemoverTelefoneAsync(Guid vendedorId, Guid telefoneId)
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Telefones
            var vendedor = await _vendedorRepository.ObterVendedorComTelefones(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var telefoneRemovido = vendedor.RemoverTelefone(telefoneId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _vendedorRepository.RemoverTelefoneAsync(vendedor, telefoneRemovido);

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
        /// Adiciona um contato para o vendedor especificado.
        /// </summary>
        public async Task AdicionarContatoAsync(
            Guid vendedorId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Contatos
            var vendedor = await _vendedorRepository.ObterVendedorComContatos(vendedorId); // Usar o método específico

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o contato (e o evento)
                var novoContato = vendedor.AdicionarContato(
                    nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _vendedorRepository.AdicionarContatoAsync(vendedor, novoContato);

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
        /// Atualiza um contato existente do vendedor especificado.
        /// </summary>
        public async Task AtualizarContatoAsync(
            Guid vendedorId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Contatos
            var vendedor = await _vendedorRepository.ObterVendedorComContatos(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o contato (e adicionar evento)
                // A atualização ocorre na entidade Contato rastreada dentro da coleção de Vendedor.
                vendedor.AtualizarContato(
                    contatoId, nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. NÃO é necessário chamar _vendedorRepository.UpdateAsync(vendedor) ou
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
        /// Remove um contato do vendedor especificado.
        /// </summary>
        public async Task RemoverContatoAsync(Guid vendedorId, Guid contatoId)
        {
            // 1. Carrega o agregado Vendedor, incluindo a coleção de Contatos
            var vendedor = await _vendedorRepository.ObterVendedorComContatos(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var contatoRemovido = vendedor.RemoverContato(contatoId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _vendedorRepository.RemoverContatoAsync(vendedor, contatoRemovido);

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
        /// Adiciona um endereço para o vendedor especificado.
        /// </summary>
        public async Task AdicionarEnderecoAsync(
            Guid vendedorId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            var vendedor = await _vendedorRepository.ObterVendedorComEnderecos(vendedorId); // Carrega com a coleção de endereços

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                var novoEndereco = vendedor.AdicionarEndereco(
                    paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

                await _vendedorRepository.AdicionarEnderecoAsync(vendedor, novoEndereco);
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
        /// Atualiza um endereço existente do vendedor especificado.
        /// </summary>
        public async Task AtualizarEnderecoAsync(
            Guid vendedorId,
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
            var vendedor = await _vendedorRepository.ObterVendedorComEnderecos(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                vendedor.AtualizarEndereco(
                    enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {                
                Notificar("Ocorreu um erro inesperado ao atualizar o endereço.");
            }
        }

        /// <summary>
        /// Remove um endereço do vendedor especificado.
        /// </summary>
        public async Task RemoverEnderecoAsync(Guid vendedorId, Guid enderecoId)
        {
            var vendedor = await _vendedorRepository.ObterVendedorComEnderecos(vendedorId);

            if (vendedor is null)
            {
                Notificar($"Vendedor com ID {vendedorId} não encontrado.");
                return;
            }

            try
            {
                var enderecoRemovido = vendedor.RemoverEndereco(enderecoId);
                await _vendedorRepository.RemoverEnderecoAsync(vendedor, enderecoRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o endereço.");
            }
        }

        #endregion


        #region Validações Privadas
        private async Task<bool> ValidarSalvarVendedor(Vendedor model, bool isUpdate)
        {
            if (!ExecutarValidacao(new VendedorValidation(), model))
                return false;

            var vendedorExistente = await _query.SearchAsync(c =>
                (c.Documento == model.Documento || c.Email == model.Email) && c.Id != model.Id);

            if (vendedorExistente.Any())
            {
                if (vendedorExistente.Any(c => c.Documento == model.Documento))
                    Notificar("O Documento informado já está em uso por outro vendedor.");
                if (vendedorExistente.Any(c => c.Email == model.Email))
                    Notificar("O E-mail informado já está em uso por outro vendedor.");
            }

            if (!isUpdate && model.StatusDoVendedor == StatusDoVendedor.Inativo)
            {
                Notificar("Nenhum vendedor pode ser cadastrado como 'Inativo'.");
            }

            return !TemNotificacao();
        }

        private async Task<bool> ValidarRemocaoVendedor(Vendedor model)
        {
            if (model.Pedidos.Any())
            {
                Notificar("O vendedor possui pedidos associados e não pode ser excluído. Considere inativá-lo.");
            }
            return !TemNotificacao();
        }

        #endregion


    }
}