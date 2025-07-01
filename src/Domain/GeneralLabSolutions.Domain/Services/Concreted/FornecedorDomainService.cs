using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class FornecedorDomainService : BaseService, IFornecedorDomainService
    {

        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IGenericRepository<Fornecedor, Guid> _query;
        private readonly IGenericRepository<PedidoDeCompra, Guid> _pedidoDeCompraQuery;
        private readonly IMediatorHandler _mediatorHandler;

        /// <summary>
        /// Construtor do serviço de domínio para Fornecedor.
        /// </summary>
        /// <param name="fornecedorRepository"></param>
        /// <param name="notificador"></param>
        /// <param name="query"></param>
        /// <param name="mediatorHandler"></param>
        public FornecedorDomainService(
            IFornecedorRepository fornecedorRepository,
            INotificador notificador,
            IGenericRepository<Fornecedor, Guid> query,
            IGenericRepository<PedidoDeCompra, Guid> pedidoDeCompraQuery,
            IMediatorHandler mediatorHandler)
            : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _query = query;
            _pedidoDeCompraQuery = pedidoDeCompraQuery;
            this._mediatorHandler = mediatorHandler;
        }

        #region: Métodos de Ação de Fornecedor

        public async Task AddFornecedorAsync(Fornecedor model)
        {
            if (!await ValidarSalvarFornecedor(model, isUpdate: false))
                return;
            await _fornecedorRepository.AddAsync(model);


            await _fornecedorRepository.AddAsync(model);

            model.AdicionarEvento(new FornecedorRegistradoEvent(
                    model.Id,
                    model.Nome,
                    model.Documento,
                    model.TipoDePessoa,
                    model.PessoaId,
                    model.StatusDoFornecedor,
                    model.Email
                ));

            // PersistirDados // Já está sendo feito no AppDbContext
        }

        public async Task UpdateFornecedorAsync(Fornecedor model)
        {
            if (!await ValidarSalvarFornecedor(model, isUpdate: true))
                return;

            // Não precisa mais disso, pois a entidade já está sendo rastreada
            // e as propriedades já foram atualizadas no controller.

            model.AdicionarEvento(new FornecedorAtualizadoEvent(
                    model.Id,
                    model.Nome,
                    model.Documento,
                    model.TipoDePessoa,
                    model.PessoaId,
                    model.StatusDoFornecedor,
                    model.Email
                ));
        }

        public async Task DeleteFornecedorAsync(Fornecedor model)
        {
            if (!await ValidarRemocaoFornecedor(model))
                return;

            await _fornecedorRepository.DeleteAsync(model);

            model.AdicionarEvento(new FornecedorDeletadoEvent(model.Id, model.Nome));

            // Publica o evento *ANTES* de qualquer operação de persistência
            await _mediatorHandler.PublicarEvento(new FornecedorDeletadoEvent(model.Id, model.Nome));

            // PersistirDados // Já está sendo feito no AppDbContext
        }

        #endregion


        #region: Métodos para ação de agregados (Dados Bancários, Contatos, Endereços, Telefones)

        public async Task AdicionarContatoAsync(Guid fornecedorId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "")
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var novoContato = fornecedor.AdicionarContato(nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
                await _fornecedorRepository.AdicionarContatoAsync(fornecedor, novoContato);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar o contato.");
            }
        }

        public async Task AdicionarDadosBancariosAsync(Guid fornecedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var novoDb = fornecedor.AdicionarDadosBancarios(banco, agencia, conta, tipoConta);
                await _fornecedorRepository.AdicionarDadosBancariosAsync(fornecedor, novoDb);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar os dados bancários.");
            }
        }

        public async Task AdicionarEnderecoAsync(Guid fornecedorId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var novoEndereco = fornecedor.AdicionarEndereco(paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco, linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
                await _fornecedorRepository.AdicionarEnderecoAsync(fornecedor, novoEndereco);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar o endereço.");
            }
        }

        public async Task AdicionarTelefoneAsync(Guid fornecedorId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var novoTelefone = fornecedor.AdicionarTelefone(ddd, numero, tipoTelefone);
                await _fornecedorRepository.AdicionarTelefoneAsync(fornecedor, novoTelefone);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar o telefone.");
            }
        }

        public async Task AtualizarContatoAsync(Guid fornecedorId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "")
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                fornecedor.AtualizarContato(contatoId, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao atualizar o contato.");
            }
        }

        public async Task AtualizarDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor == null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                fornecedor.AtualizarDadosBancarios(dadosBancariosId, banco, agencia, conta, tipoConta);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao atualizar os dados bancários.");
            }
        }

        public async Task AtualizarEnderecoAsync(Guid fornecedorId, Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                fornecedor.AtualizarEndereco(enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco, linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao atualizar o endereço.");
            }
        }

        public async Task AtualizarTelefoneAsync(Guid fornecedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                fornecedor.AtualizarTelefone(telefoneId, ddd, numero, tipoTelefone);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao atualizar o telefone.");
            }
        }



        public async Task RemoverContatoAsync(Guid fornecedorId, Guid contatoId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var contatoRemovido = fornecedor.RemoverContato(contatoId);
                await _fornecedorRepository.RemoverContatoAsync(fornecedor, contatoRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao remover o contato.");
            }
        }

        public async Task RemoverDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var removido = fornecedor.RemoverDadosBancarios(dadosBancariosId);
                await _fornecedorRepository.RemoverDadosBancariosAsync(fornecedor, removido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                Notificar("Ocorreu um erro inesperado ao remover os dados bancários.");
            }
        }

        public async Task RemoverEnderecoAsync(Guid fornecedorId, Guid enderecoId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var enderecoRemovido = fornecedor.RemoverEndereco(enderecoId);
                await _fornecedorRepository.RemoverEnderecoAsync(fornecedor, enderecoRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao remover o endereço.");
            }
        }

        public async Task RemoverTelefoneAsync(Guid fornecedorId, Guid telefoneId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
            {
                Notificar($"Fornecedor com ID {fornecedorId} não encontrado.");
                return;
            }

            try
            {
                var telefoneRemovido = fornecedor.RemoverTelefone(telefoneId);
                await _fornecedorRepository.RemoverTelefoneAsync(fornecedor, telefoneRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                Notificar("Ocorreu um erro inesperado ao remover o telefone.");
            }
        }


        #endregion



        #region Validações Privadas
        private async Task<bool> ValidarSalvarFornecedor(Fornecedor model, bool isUpdate)
        {
            if (!ExecutarValidacao(new FornecedorValidation(), model))
                return false;

            var fornecedorExistente = await _query.SearchAsync(c =>
                (c.Documento == model.Documento || c.Email == model.Email) && c.Id != model.Id);

            if (fornecedorExistente.Any())
            {
                if (fornecedorExistente.Any(c => c.Documento == model.Documento))
                    Notificar("O Documento informado já está em uso por outro fornecedor.");
                if (fornecedorExistente.Any(c => c.Email == model.Email))
                    Notificar("O E-mail informado já está em uso por outro fornecedor.");
            }

            if (!isUpdate && model.StatusDoFornecedor == StatusDoFornecedor.Inativo)
            {
                Notificar("Nenhum fornecedor pode ser cadastrado como 'Inativo'.");
            }

            return !TemNotificacao();
        }

        private async Task<bool> ValidarRemocaoFornecedor(Fornecedor model)
        {
            // Inclui a verificação de produtos que já existia
            if (model.Produtos.Any())
            {
                Notificar("O fornecedor possui produtos associados e não pode ser excluído. Considere inativá-lo.");
            }

            // A verificação de PedidoDeCompra precisaria que o repositório fosse injetado
            // ou que a relação estivesse na entidade Fornecedor.
            //var pedidosDeCompra = await _pedidoDeCompraQuery.SearchAsync(pc => pc.FornecedorId == model.Id);
            //if (pedidosDeCompra.Any())
            //{
            //    Notificar("O fornecedor possui pedidos de compra e não pode ser excluído.");
            //}

            return !TemNotificacao();
        }
        #endregion


    }
}
