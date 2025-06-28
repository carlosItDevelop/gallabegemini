using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    //[Authorize]
    public class CategoriaProdutoController : BaseMvcController
    {

        private readonly IMapper _mapper;

        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICategoriaDomainService _categoriaDomainService;
        private readonly IGenericRepository<CategoriaProduto, Guid> _query;


        public CategoriaProdutoController(IMapper mapper, 
                            ICategoriaRepository categoriaRepository, 
                            ICategoriaDomainService categoriaDomainService,         IGenericRepository<CategoriaProduto, Guid> query,
                            INotificador notificador) : base(notificador)
        {
            _mapper = mapper;
            _categoriaRepository = categoriaRepository;
            _categoriaDomainService = categoriaDomainService;
            _query = query;
        }

        //[AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var listaCategoriaProdutoViewModel 
                = _mapper.Map<IEnumerable<CategoriaProdutoViewModel>>(await _query.GetAllAsync());

            return View(listaCategoriaProdutoViewModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var categoriaProdutoViewModel
                = _mapper.Map<CategoriaProdutoViewModel>(await _query.GetByIdAsync(id));

            if (categoriaProdutoViewModel == null)
            {
                return NotFound();
            }


            return View(categoriaProdutoViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProdutoViewModel CategoriaViewModel)
        {
            if (ModelState.IsValid)
            {
                await _categoriaDomainService.AddCategoriaAsync(_mapper.Map<CategoriaProduto>(CategoriaViewModel));

                await _categoriaRepository.UnitOfWork.CommitAsync();
                TempData ["Success"] = "Categoria adicionada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(CategoriaViewModel);
        }

        public async Task<IActionResult> Edit(Guid id)
        {

            var categoriaProdutoViewModel
                = _mapper.Map<CategoriaProdutoViewModel>(await _query.GetByIdAsync(id));

            if (categoriaProdutoViewModel == null)
            {
                return NotFound();
            }
            return View(categoriaProdutoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoriaProdutoViewModel categoriaViewModel)
        {
            if (id != categoriaViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriaDomainService.UpdateCategoriaAsync(_mapper.Map<CategoriaProduto>(categoriaViewModel));

                    // Mostra notificações de Validação de regra de negócio,
                    // caso haja e NÃO deixa que o CommitAsync seja chamado.
                    if (!OperacaoValida())
                        return RedirectToAction(nameof(Index));


                    await _categoriaRepository.UnitOfWork.CommitAsync();
                    TempData ["Success"] = "Categoria atualizada com sucesso!";


                } catch (DbUpdateConcurrencyException)
                {
                    if (!await CategotiaExists(categoriaViewModel.Id))
                    {
                        return NotFound();
                    } else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(categoriaViewModel);
        }

        public async Task<IActionResult> Delete(Guid id)
        {


            var categoria
                = await _query.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            return View(_mapper.Map<CategoriaProdutoViewModel>(await _query.GetByIdAsync(id)));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var categoria = await _query.GetByIdAsync(id);

            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }
            await _categoriaDomainService.DeleteCategoriaProdutoAsync(categoria);

            // Mostra notificações de Validação de regra de negócio,
            // caso haja e NÃO deixa que o CommitAsync seja chamado.
            if (!OperacaoValida())
                return RedirectToAction(nameof(Index));


            await _categoriaRepository.UnitOfWork.CommitAsync();

            TempData ["Success"] = "Categoria excluída com sucesso!";

            return RedirectToAction(nameof(Index));

        }

        private async Task<bool> CategotiaExists(Guid id)
        {
            return await _categoriaRepository.TemCategoria(id);

        }
    }
}