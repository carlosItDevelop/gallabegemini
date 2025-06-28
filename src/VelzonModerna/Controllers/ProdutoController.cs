using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Controllers.Base;

namespace VelzonModerna.Controllers
{
    // Todo: Implementar Repository Pattern
    public class ProdutoController : BaseMvcController
    {
        private readonly AppDbContext _context;

        private readonly IProdutoRepository _produtoRepository;

        public ProdutoController(AppDbContext context, 
                                 IProdutoRepository produtoRepository, 
                                 INotificador notificador) : base(notificador)
        {
            _context = context;
            _produtoRepository = produtoRepository;
        }

        [Route("lista-de-produtos")]
        public async Task<IActionResult> Index()
        {
            return View(await _produtoRepository.GetAllProductWithIncludesAsync());
        }

        [Route("detalhes-do-produto/{id:guid}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.CategoriaProduto)
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        [Route("adicionar-produto")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaProduto, "Id", "Descricao");
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome");
            return View();
        }

        [Route("adicionar-produto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaProduto, "Id", "Descricao", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);

            TempData["Erros"] = ModelState.Values.SelectMany(v=>v.Errors.Select(e=>e.ErrorMessage)).ToList();
            return View(produto);
        }

        [Route("atualizar-produto/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaProduto, "Id", "Descricao", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);
            return View(produto);
        }

        
        [HttpPost]
        [Route("atualizar-produto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                } catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData ["CategoriaId"] = new SelectList(_context.CategoriaProduto, "Id", "Descricao", produto.CategoriaId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);


            return View(produto);
        }

        [Route("excluir-produto/{id:guid}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.CategoriaProduto)
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        [Route("excluir-produto/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produto = await _context.Produto.FindAsync(id);
            if (produto != null)
            {
                _context.Produto.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProdutoExists(Guid id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }
    }
}
