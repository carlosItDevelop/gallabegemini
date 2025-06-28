using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Services.Concreted;
using GeneralLabSolutions.Domain.Services.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var model = await _fornecedorRepository.GetByIdAsync(id);

            if (model == null)
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
            var model = await _fornecedorRepository.GetByIdAsync(id);

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
            if (model == null)
                return NotFound();
            var viewModel = _mapper.Map<FornecedorViewModel>(model);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var model = await _fornecedorRepository.GetByIdAsync(id);
            if (model == null)
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




    }
}
