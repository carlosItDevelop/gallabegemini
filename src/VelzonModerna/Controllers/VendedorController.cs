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
    public class VendedorController : BaseMvcController
    {
        private readonly IVendedorDomainService _vendedorDomainService;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IMapper _mapper;
        private readonly INotificador _notificador;

        public VendedorController(
            IVendedorDomainService vendedorDomainService,
            IVendedorRepository vendedorRepository,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            _vendedorDomainService = vendedorDomainService;
            _vendedorRepository = vendedorRepository;
            _mapper = mapper;
            _notificador = notificador;
        }

        // GET: Vendedor
        public async Task<IActionResult> Index()
        {
            var vendedores = await _vendedorRepository.GetAllAsync();
            var listaVendedorViewModel = _mapper.Map<IEnumerable<VendedorViewModel>>(vendedores);
            return View(listaVendedorViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {

            var model = await _vendedorRepository.ObterVendedorCompleto(id); // Carregar o vendedor completo com dados bancários, telefones, contatos e endereços
            if (model is null)
            {
                return NotFound("Vendedor não pode ser Nulo.");
            }

            var vModel = _mapper.Map<VendedorViewModel>(model);

            return View(vModel);
        }

        [HttpGet]
        public IActionResult Create() => View(new VendedorViewModel());


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendedorViewModel vendedorViewModel)
        {
            if (!ModelState.IsValid)
                return View(vendedorViewModel);

            var vendedor = _mapper.Map<Vendedor>(vendedorViewModel);

            await _vendedorDomainService.AddVendedorAsync(vendedor);

            if (!OperacaoValida())
            {
                return View(vendedorViewModel);
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados.");
                return View(vendedorViewModel);
            }

            TempData ["Success"] = "Vendedor Adicionado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vendedor = await _vendedorRepository.ObterVendedorCompleto(id); // Carregar o vendedor completo com dados bancários, telefones, contatos e endereços
            if (vendedor is null)
            {
                return NotFound("Vendedor não encontrado.");
            }

            var vendedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);

            return View(vendedorViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VendedorViewModel vendedorViewModel)
        {
            if (id != vendedorViewModel.Id)
                return NotFound("Parâmetro ID não confere com o ID da ViewModel");

            if (!ModelState.IsValid)
                return View(vendedorViewModel);

            var vendedorCompleto = await _vendedorRepository.ObterVendedorCompleto(id);
            if (vendedorCompleto is null)
            {
                NotificarErro("Vendedor não encontrado.");
                return View(vendedorViewModel);
            }

            // ✅ Copia os dados do ViewModel para a entidade já rastreada pelo contexto
            _mapper.Map(vendedorViewModel, vendedorCompleto);

            await _vendedorDomainService.UpdateVendedorAsync(vendedorCompleto);

            if (!OperacaoValida())
                return View(vendedorViewModel);

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados.");
                var vendedorAtualizado = await _vendedorRepository.ObterVendedorCompleto(id);
                var viewModelAtualizada = _mapper.Map<VendedorViewModel>(vendedorAtualizado);
                return View(viewModelAtualizada);     // 👈 devolve ViewModel, não entidade
            }

            TempData ["Success"] = "Vendedor atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);
            if (vendedor is null) return NotFound("Vendedor não encontrado.");

            var vendedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);

            return View(vendedorViewModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);

            // CORREÇÃO 1: Usar NotFound() para consistência.
            if (vendedor is null)
            {
                return NotFound();
            }

            // O serviço já foi corrigido para receber a entidade.
            await _vendedorDomainService.DeleteVendedorAsync(vendedor);

            if (!OperacaoValida())
            {
                // CORREÇÃO 2: Corrigido o typo na variável.
                var vendedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);

                // CORREÇÃO 3: Retornar para a view "Delete" com o modelo para exibir os erros.
                return View("Delete", vendedorViewModel);
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir os dados do vendedor.");
                var vendedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);
                return View("Delete", vendedorViewModel);
            }

            TempData ["Success"] = "Vendedor Excluído com Sucesso!";
            return RedirectToAction(nameof(Index));
        }



        // Cole este bloco de código dentro da classe VelzonModerna.Controllers.VendedorController

        #region Actions AJAX para Dados Bancários

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosListPartial(Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
            if (vendedor is null || vendedor.Pessoa is null)
            {
                return PartialView("PartialViews/_DadosBancariosListVendedorPartial", new List<DadosBancariosViewModel>());
            }
            var viewModels = _mapper.Map<List<DadosBancariosViewModel>>(vendedor.Pessoa.DadosBancarios);
            ViewData ["PessoaId"] = vendedor.PessoaId;
            return PartialView("PartialViews/_DadosBancariosListVendedorPartial", viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosFormData(Guid? dadosBancariosId, Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null)
                return NotFound("Vendedor não encontrado.");

            if (dadosBancariosId is null || dadosBancariosId == Guid.Empty)
            {
                var newViewModel = new DadosBancariosViewModel { PessoaId = vendedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var vendedorCompleto = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
                var dadosBancarios = vendedorCompleto?.Pessoa?.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId.Value);

                if (dadosBancarios is null)
                    return NotFound("Dados bancários não encontrados ou não pertencem a este vendedor.");

                var viewModel = _mapper.Map<DadosBancariosViewModel>(dadosBancarios);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarDadosBancarios(Guid vendedorId, DadosBancariosViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (vendedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Vendedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _vendedorDomainService.AdicionarDadosBancariosAsync(vendedorId, viewModel.Banco, viewModel.Agencia, viewModel.Conta, viewModel.TipoDeContaBancaria);
            } else
            {
                await _vendedorDomainService.AtualizarDadosBancariosAsync(vendedorId, viewModel.Id, viewModel.Banco, viewModel.Agencia, viewModel.Conta, viewModel.TipoDeContaBancaria);
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar os dados no banco." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirDadosBancarios(Guid vendedorId, Guid dadosBancariosId)
        {
            if (vendedorId == Guid.Empty || dadosBancariosId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _vendedorDomainService.RemoverDadosBancariosAsync(vendedorId, dadosBancariosId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir os dados do banco." } });
            }

            return Json(new { success = true });
        }

        #endregion

        #region Actions AJAX para Telefones

        [HttpGet]
        public async Task<IActionResult> GetTelefonesListPartial(Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
            if (vendedor is null || vendedor.Pessoa is null)
            {
                return PartialView("PartialViews/_TelefonesListVendedorPartial", new List<TelefoneViewModel>());
            }
            var viewModels = _mapper.Map<List<TelefoneViewModel>>(vendedor.Pessoa.Telefones);
            ViewData ["PessoaId"] = vendedor.PessoaId;
            return PartialView("PartialViews/_TelefonesListVendedorPartial", viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetTelefoneFormData(Guid? telefoneId, Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null)
                return NotFound("Vendedor não encontrado.");

            if (telefoneId is null || telefoneId == Guid.Empty)
            {
                var newViewModel = new TelefoneViewModel { PessoaId = vendedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var vendedorCompleto = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
                var telefone = vendedorCompleto?.Pessoa?.Telefones.FirstOrDefault(t => t.Id == telefoneId.Value);

                if (telefone is null)
                    return NotFound("Telefone não encontrado ou não pertence a este vendedor.");

                var viewModel = _mapper.Map<TelefoneViewModel>(telefone);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTelefone(Guid vendedorId, TelefoneViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (vendedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Vendedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _vendedorDomainService.AdicionarTelefoneAsync(
                    vendedorId,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            } else
            {
                await _vendedorDomainService.AtualizarTelefoneAsync(
                    vendedorId,
                    viewModel.Id,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o telefone." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTelefone(Guid vendedorId, Guid telefoneId)
        {
            if (vendedorId == Guid.Empty || telefoneId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _vendedorDomainService.RemoverTelefoneAsync(vendedorId, telefoneId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o telefone." } });
            }

            return Json(new { success = true });
        }

        #endregion

        #region Actions AJAX para Contatos

        [HttpGet]
        public async Task<IActionResult> GetContatosListPartial(Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
            if (vendedor is null || vendedor.Pessoa is null)
            {
                return PartialView("PartialViews/_ContatosListVendedorPartial", new List<ContatoViewModel>());
            }
            var viewModels = _mapper.Map<List<ContatoViewModel>>(vendedor.Pessoa.Contatos);
            ViewData ["PessoaId"] = vendedor.PessoaId;
            return PartialView("PartialViews/_ContatosListVendedorPartial", viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetContatoFormData(Guid? contatoId, Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null)
                return NotFound("Vendedor não encontrado.");

            if (contatoId is null || contatoId == Guid.Empty)
            {
                var newViewModel = new ContatoViewModel { PessoaId = vendedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var vendedorCompleto = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
                var contato = vendedorCompleto?.Pessoa?.Contatos.FirstOrDefault(c => c.Id == contatoId.Value);

                if (contato is null)
                    return NotFound("Contato não encontrado ou não pertence a este vendedor.");

                var viewModel = _mapper.Map<ContatoViewModel>(contato);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarContato(Guid vendedorId, ContatoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (vendedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Vendedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _vendedorDomainService.AdicionarContatoAsync(
                    vendedorId,
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao);
            } else
            {
                await _vendedorDomainService.AtualizarContatoAsync(
                    vendedorId,
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

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o contato." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirContato(Guid vendedorId, Guid contatoId)
        {
            if (vendedorId == Guid.Empty || contatoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _vendedorDomainService.RemoverContatoAsync(vendedorId, contatoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o contato." } });
            }

            return Json(new { success = true });
        }

        #endregion

        #region Actions AJAX para Endereços

        [HttpGet]
        public async Task<IActionResult> GetEnderecosListPartial(Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
            if (vendedor is null || vendedor.Pessoa is null)
            {
                return PartialView("PartialViews/_EnderecosListVendedorPartial", new List<EnderecoViewModel>());
            }
            var viewModels = _mapper.Map<List<EnderecoViewModel>>(vendedor.Pessoa.Enderecos);
            ViewData ["PessoaId"] = vendedor.PessoaId;
            return PartialView("PartialViews/_EnderecosListVendedorPartial", viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetEnderecoFormData(Guid? enderecoId, Guid vendedorId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor is null)
                return NotFound("Vendedor não encontrado.");

            if (enderecoId is null || enderecoId == Guid.Empty)
            {
                var newViewModel = new EnderecoViewModel { PessoaId = vendedor.PessoaId };
                return Json(newViewModel);
            } else
            {
                var vendedorCompleto = await _vendedorRepository.ObterVendedorCompleto(vendedorId);
                var endereco = vendedorCompleto?.Pessoa?.Enderecos.FirstOrDefault(e => e.Id == enderecoId.Value);

                if (endereco is null)
                    return NotFound("Endereço não encontrado ou não pertence a este vendedor.");

                var viewModel = _mapper.Map<EnderecoViewModel>(endereco);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEndereco(Guid vendedorId, EnderecoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (vendedorId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Vendedor não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _vendedorDomainService.AdicionarEnderecoAsync(
                    vendedorId,
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
                await _vendedorDomainService.AtualizarEnderecoAsync(
                    vendedorId,
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

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro ao salvar o endereço." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEndereco(Guid vendedorId, Guid enderecoId)
        {
            if (vendedorId == Guid.Empty || enderecoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _vendedorDomainService.RemoverEnderecoAsync(vendedorId, enderecoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
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
