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
using GeneralLabSolutions.Domain.Services.Interfaces;
using GeneralLabSolutions.Domain.Validations;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class FornecedorDomainService : BaseService, IFornecedorDomainService
    {

        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMediatorHandler _mediatorHandler;

        /// <summary>
        /// Construtor do serviço de domínio para Fornecedor.
        /// </summary>
        /// <param name="fornecedorRepository"></param>
        /// <param name="notificador"></param>
        /// <param name="mediatorHandler"></param>
        public FornecedorDomainService(
            IFornecedorRepository fornecedorRepository,
            INotificador notificador,
            IMediatorHandler mediatorHandler)
            : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            this._mediatorHandler = mediatorHandler;
        }


        #region: Regras de Negócios Agrupadas para evitar várias interrupções: Add

        public async Task<bool> ValidarAddFornecedor(Fornecedor model) 
        {
            bool isValid = true;

            var fornecedores = await _fornecedorRepository.SearchAsync(c =>
                c.Documento == model.Documento || c.Email == model.Email);

            if (fornecedores.Any(c => c.Documento == model.Documento))
            {
                Notificar("Já existe um Fornecedor com este documento informado.");
                isValid = false;
            }

            if (fornecedores.Any(c => c.Email == model.Email))
            {
                Notificar("Já existe um Fornecedor com este Email. Tente outro!");
                isValid = false;
            }

            if (model.StatusDoFornecedor == StatusDoFornecedor.Inativo)
            {
                Notificar("Nenhum fornecedor pode ser Adicionado com o Status de 'Inativo'.");
                isValid = false;
            }

            var fornecedorExiste = await _fornecedorRepository.GetByIdAsync(model.Id);
            if (fornecedorExiste is not null)
            {
                Notificar("Já existe um Fornecedor cadastrado com este ID.");
                isValid = false;
            }

            if (!ExecutarValidacao(new FornecedorValidation(), model))
            {
                isValid = false;
            }

            return isValid;
        }


        public async Task<bool> ValidarDelFornecedor(Fornecedor model)
        {
            bool isValid = true;

            // Todo: E se, ao invés de excluirmos apenas deixá-lo "Inativo"?
            if (!ExecutarValidacao(new FornecedorValidation(), model))
                isValid = false;

            return isValid;
        }

        public async Task<bool> ValidarUpdFornecedor(Fornecedor model)
        {
            bool isValid = true;

            if (!ExecutarValidacao(new FornecedorValidation(), model))
                isValid = false;

            return isValid;
        }

        #endregion



        #region: Métodos de Ação de Fornecedor

        public async Task AddFornecedorAsync(Fornecedor model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarAddFornecedor(model))
                return;


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

        public async Task DeleteFornecedorAsync(Fornecedor model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarDelFornecedor(model))
                return;

            await _fornecedorRepository.DeleteAsync(model);

            model.AdicionarEvento(new FornecedorDeletadoEvent(model.Id, model.Nome));

            // PersistirDados // Já está sendo feito no AppDbContext
        }

        public async Task UpdateFornecedorAsync(Fornecedor model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarUpdFornecedor(model))
                return;

            await _fornecedorRepository.UpdateAsync(model);

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

        #endregion


        #region: Métodos para ação de agregados (Dados Bancários, Contatos, Endereços, Telefones)

        public Task AdicionarContatoAsync(Guid fornecedorId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "")
        {
            throw new NotImplementedException();
        }

        public Task AdicionarDadosBancariosAsync(Guid fornecedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            throw new NotImplementedException();
        }

        public Task AdicionarEnderecoAsync(Guid fornecedorId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            throw new NotImplementedException();
        }

        public Task AdicionarTelefoneAsync(Guid fornecedorId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            throw new NotImplementedException();
        }

        public Task AtualizarContatoAsync(Guid fornecedorId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlternativo = "", string telefoneAlternativo = "", string observacao = "")
        {
            throw new NotImplementedException();
        }

        public Task AtualizarDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            throw new NotImplementedException();
        }

        public Task AtualizarEnderecoAsync(Guid fornecedorId, Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            throw new NotImplementedException();
        }

        public Task AtualizarTelefoneAsync(Guid fornecedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            throw new NotImplementedException();
        }



        public Task RemoverContatoAsync(Guid fornecedorId, Guid contatoId)
        {
            throw new NotImplementedException();
        }

        public Task RemoverDadosBancariosAsync(Guid fornecedorId, Guid dadosBancariosId)
        {
            throw new NotImplementedException();
        }

        public Task RemoverEnderecoAsync(Guid fornecedorId, Guid enderecoId)
        {
            throw new NotImplementedException();
        }

        public Task RemoverTelefoneAsync(Guid fornecedorId, Guid telefoneId)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
