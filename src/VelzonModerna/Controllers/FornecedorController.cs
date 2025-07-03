using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class FornecedorController : BaseMvcController
    {

        private readonly IMapper _mapper;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorDomainService _fornecedorDomainService;

        public FornecedorController(
            INotificador notificador,
            IMapper mapper,
            IFornecedorRepository fornecedorRepository,
            IFornecedorDomainService fornecedorDomainService)
            : base(notificador)
        {
            _mapper = mapper;
            _fornecedorRepository = fornecedorRepository;
            _fornecedorDomainService = fornecedorDomainService;
        }

        #region: Métodos Cliente (Index, Create, Edit, Delete - Ajustar Details/Edit GET)

        public async Task<IActionResult> Index()
        {
            var fornecedores = await _fornecedorRepository.GetAllAsync();
            var listViewModel = _mapper.Map<IEnumerable<FornecedorViewModel>>(fornecedores);
            return View(listViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _fornecedorRepository.ObterFornecedorCompleto(id);

            if (model is null)
                return NotFound();
            var viewModel = _mapper.Map<FornecedorViewModel>(model);
            return View(viewModel);
        }

        public IActionResult Create() => View(new FornecedorViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FornecedorViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var model = _mapper.Map<Fornecedor>(viewModel);
            await _fornecedorDomainService.AddFornecedorAsync(model);
            if (!OperacaoValida())
                return View(viewModel);
            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados.");
                return View(viewModel);
            }
            TempData ["Success"] = "Fornecedor Adicionado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _fornecedorRepository.ObterFornecedorCompleto(id);

            if (model == null)
                return NotFound();
            var viewModel = _mapper.Map<FornecedorViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(viewModel);

            var model = await _fornecedorRepository.GetByIdAsync(id);
            if (model is null)
                return NotFound();

            _mapper.Map(viewModel, model);

            await _fornecedorDomainService.UpdateFornecedorAsync(model);

            if (!OperacaoValida())
                return View(viewModel);
            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao atualizar os dados do fornecedor.");
                var modelAtualizado = await _fornecedorRepository.GetByIdAsync(id);

                var viewModelAtualizado = _mapper.Map<FornecedorViewModel>(modelAtualizado);
                return View(viewModelAtualizado);
            }
            TempData ["Success"] = "Fornecedor Atualizado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _fornecedorRepository.GetByIdAsync(id);
            if (model is null)
                return NotFound();
            var viewModel = _mapper.Map<FornecedorViewModel>(model);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var model = await _fornecedorRepository.GetByIdAsync(id);
            if (model is null)
                return NotFound();
            await _fornecedorDomainService.DeleteFornecedorAsync(model);
            if (!OperacaoValida())
            {
                var viewModel = _mapper.Map<FornecedorViewModel>(model);
                return View(viewModel); // Retorna para a view Delete com erros
            }
            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir o fornecedor. Verifique dependências.");
                var viewModel = _mapper.Map<FornecedorViewModel>(model);
                return View("Delete", viewModel);
            }
            TempData ["Success"] = "Fornecedor excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Actions AJAX para Dados Bancários

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosListPartial(Guid fornecedorId)
        {
            try
            {
                var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

                // CORREÇÃO: Primeiro, obtemos a lista de entidades de domínio, garantindo que não seja nula.
                var dadosBancariosEntities = fornecedor?.Pessoa?.DadosBancarios ?? new List<DadosBancarios>();

                // Segundo, mapeamos a lista de entidades para uma lista de ViewModels.
                var dadosBancariosViewModels = _mapper.Map<List<DadosBancariosViewModel>>(dadosBancariosEntities);

                // Terceiro, passamos a lista de ViewModels para o ViewComponent.
                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Fornecedor",
                    parentEntityId = fornecedorId,
                    parentPessoaId = fornecedor?.PessoaId ?? Guid.Empty,
                    aggregateType = "DadosBancarios",
                    items = dadosBancariosViewModels // Agora a lista tem o tipo correto
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os dados bancários: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosFormData(Guid? dadosBancariosId, Guid fornecedorId)
        {
            // Valida se o fornecedor principal existe.
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
            if (fornecedor is null)
                return NotFound("Fornecedor não encontrado.");

            // Se não há um ID de dados bancários, estamos criando um novo.
            if (dadosBancariosId is null || dadosBancariosId == Guid.Empty)
            {
                // Retorna um ViewModel vazio, mas com o PessoaId do fornecedor já preenchido.
                var newViewModel = new DadosBancariosViewModel { PessoaId = fornecedor.PessoaId };
                return Json(newViewModel);
            } else // Caso contrário, estamos editando um existente.
            {
                // Busca os dados bancários específicos.
                // A melhor forma é buscar através do agregado para garantir o pertencimento.
                var fornecedorCompleto = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
                var dadosBancarios = fornecedorCompleto?.Pessoa?.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId.Value);

                if (dadosBancarios is null)
                    return NotFound("Dados bancários não encontrados ou não pertencem a este fornecedor.");

                var viewModel = _mapper.Map<DadosBancariosViewModel>(dadosBancarios);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarDadosBancarios(Guid fornecedorId, DadosBancariosViewModel viewModel)
        {
            // 1. Validação do modelo recebido.
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            // 2. Validação do ID do fornecedor.
            if (fornecedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Fornecedor não foi fornecido." } });
            }

            // 3. Determina se é uma operação de criação ou atualização.
            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                // Chama o serviço de domínio para ADICIONAR.
                await _fornecedorDomainService.AdicionarDadosBancariosAsync(
                    fornecedorId,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            } else
            {
                // Chama o serviço de domínio para ATUALIZAR.
                await _fornecedorDomainService.AtualizarDadosBancariosAsync(
                    fornecedorId,
                    viewModel.Id,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            }

            // 4. Verifica se o serviço de domínio encontrou algum erro de regra de negócio.
            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            // 5. Tenta persistir as mudanças no banco de dados.
            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar os dados no banco." } });
            }

            // 6. Retorna sucesso.
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirDadosBancarios(Guid fornecedorId, Guid dadosBancariosId)
        {
            // 1. Validação dos IDs.
            if (fornecedorId == Guid.Empty || dadosBancariosId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            // 2. Chama o serviço de domínio para REMOVER.
            await _fornecedorDomainService.RemoverDadosBancariosAsync(fornecedorId, dadosBancariosId);

            // 3. Verifica se o serviço de domínio encontrou algum erro.
            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            // 4. Tenta persistir a exclusão no banco de dados.
            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir os dados do banco." } });
            }

            // 5. Retorna sucesso.
            return Json(new { success = true });
        }

        #endregion


        #region Actions AJAX para Telefones

        [HttpGet]
        public async Task<IActionResult> GetTelefonesListPartial(Guid fornecedorId)
        {
            try
            {
                var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

                var telefoneEntities = fornecedor?.Pessoa?.Telefones ?? new List<Telefone>();
                var telefoneViewModels = _mapper.Map<List<TelefoneViewModel>>(telefoneEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Fornecedor",
                    parentEntityId = fornecedorId,
                    parentPessoaId = fornecedor?.PessoaId ?? Guid.Empty,
                    aggregateType = "Telefone",
                    items = telefoneViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os telefones: {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetTelefoneFormData(Guid? telefoneId, Guid fornecedorId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
            if (fornecedor is null)
                return NotFound("Fornecedor não encontrado.");

            if (telefoneId is null || telefoneId == Guid.Empty)
            {
                var newViewModel = new TelefoneViewModel { PessoaId = fornecedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var fornecedorCompleto = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
                var telefone = fornecedorCompleto?.Pessoa?.Telefones.FirstOrDefault(t => t.Id == telefoneId.Value);

                if (telefone is null)
                    return NotFound("Telefone não encontrado ou não pertence a este fornecedor.");

                var viewModel = _mapper.Map<TelefoneViewModel>(telefone);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTelefone(Guid fornecedorId, TelefoneViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (fornecedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Fornecedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _fornecedorDomainService.AdicionarTelefoneAsync(
                    fornecedorId,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            } else
            {
                await _fornecedorDomainService.AtualizarTelefoneAsync(
                    fornecedorId,
                    viewModel.Id,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o telefone." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTelefone(Guid fornecedorId, Guid telefoneId)
        {
            if (fornecedorId == Guid.Empty || telefoneId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _fornecedorDomainService.RemoverTelefoneAsync(fornecedorId, telefoneId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o telefone." } });
            }

            return Json(new { success = true });
        }

        #endregion


        #region Actions AJAX para Contatos

        [HttpGet]
        public async Task<IActionResult> GetContatosListPartial(Guid fornecedorId)
        {
            try
            {

                var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

                var contatoEntities = fornecedor?.Pessoa?.Contatos ?? new List<Contato>();
                var contatoViewModels = _mapper.Map<List<ContatoViewModel>>(contatoEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Fornecedor",
                    parentEntityId = fornecedorId,
                    parentPessoaId = fornecedor?.PessoaId ?? Guid.Empty,
                    aggregateType = "Contato",
                    items = contatoViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os contatos: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetContatoFormData(Guid? contatoId, Guid fornecedorId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
            if (fornecedor == null)
                return NotFound("Fornecedor não encontrado.");

            if (contatoId is null || contatoId == Guid.Empty)
            {
                var newViewModel = new ContatoViewModel { PessoaId = fornecedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var fornecedorCompleto = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
                var contato = fornecedorCompleto?.Pessoa?.Contatos.FirstOrDefault(c => c.Id == contatoId.Value);

                if (contato is null)
                    return NotFound("Contato não encontrado ou não pertence a este fornecedor.");

                var viewModel = _mapper.Map<ContatoViewModel>(contato);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarContato(Guid fornecedorId, ContatoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (fornecedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Fornecedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _fornecedorDomainService.AdicionarContatoAsync(
                    fornecedorId,
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao);
            } else
            {
                await _fornecedorDomainService.AtualizarContatoAsync(
                    fornecedorId,
                    viewModel.Id,
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao);
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o contato." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirContato(Guid fornecedorId, Guid contatoId)
        {
            if (fornecedorId == Guid.Empty || contatoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _fornecedorDomainService.RemoverContatoAsync(fornecedorId, contatoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o contato." } });
            }

            return Json(new { success = true });
        }

        #endregion


        #region Actions AJAX para Endereços

        [HttpGet]
        public async Task<IActionResult> GetEnderecosListPartial(Guid fornecedorId)
        {

            try
            {
                var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

                var enderecoEntities = fornecedor?.Pessoa?.Enderecos ?? new List<Endereco>();
                var enderecoViewModels = _mapper.Map<List<EnderecoViewModel>>(enderecoEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Fornecedor",
                    parentEntityId = fornecedorId,
                    parentPessoaId = fornecedor?.PessoaId ?? Guid.Empty,
                    aggregateType = "Endereco",
                    items = enderecoViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetEnderecoFormData(Guid? enderecoId, Guid fornecedorId)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);

            if (fornecedor is null)
                return NotFound("Fornecedor não encontrado.");

            if (enderecoId is null || enderecoId == Guid.Empty)
            {
                var newViewModel = new EnderecoViewModel { PessoaId = fornecedor.PessoaId };
                return Json(newViewModel);

            } else
            {
                var fornecedorCompleto = await _fornecedorRepository.ObterFornecedorCompleto(fornecedorId);
                var endereco = fornecedorCompleto?.Pessoa?.Enderecos.FirstOrDefault(e => e.Id == enderecoId.Value);

                if (endereco is null)
                    return NotFound("Endereço não encontrado ou não pertence a este fornecedor.");

                var viewModel = _mapper.Map<EnderecoViewModel>(endereco);

                return Json(viewModel);

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEndereco(Guid fornecedorId, EnderecoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (fornecedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Fornecedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _fornecedorDomainService.AdicionarEnderecoAsync(
                    fornecedorId,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais);
            } else
            {
                await _fornecedorDomainService.AtualizarEnderecoAsync(
                    fornecedorId,
                    viewModel.Id,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais);
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o endereço." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEndereco(Guid fornecedorId, Guid enderecoId)
        {
            if (fornecedorId == Guid.Empty || enderecoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _fornecedorDomainService.RemoverEnderecoAsync(fornecedorId, enderecoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _fornecedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o endereço." } });
            }

            return Json(new { success = true });
        }

        #endregion




        #region Métodos Privados Auxiliares (Existente)
        //private async Task<bool> FornecedorExists(Guid id) => await _fornecedorRepository.TemFornecedor(id);

        private List<Notificacao> ObterNotificacoes() => HttpContext.RequestServices.GetRequiredService<INotificador>().ObterNotificacoes();

        #endregion



    }
}
