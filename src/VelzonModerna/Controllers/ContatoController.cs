using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ContatoController : Controller
    {
        // ToDo: Implementar Repository Pattern
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ContatoController(AppDbContext context, 
                                 IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Contato
        public async Task<IActionResult> Index()
        {
            var viewModelList = _mapper.Map<IEnumerable<ContatoViewModel>>(await _context.Contato.ToListAsync());

            return View(viewModelList);
        }

        // GET: Contato/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var viewmodel = _mapper.Map<ContatoViewModel>(await _context.Contato
                .FirstOrDefaultAsync(m => m.Id == id));

            if (viewmodel == null)
            {
                return NotFound("Modelo não encontrado!");
            }

            return View(viewmodel);
        }

        // GET: Contato/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContatoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var contato = _mapper.Map<Contato>(viewModel);

                _context.Add(contato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Contato/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {

            var viewModel = _mapper.Map<ContatoViewModel>(await _context.Contato.FindAsync(id));

            if (viewModel == null)
            {
                return NotFound("Modelo não encontrado!");
            }
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ContatoViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound("Modelo não Correspondente!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contato = _mapper.Map<Contato>(viewModel);
                    _context.Update(contato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContatoExists(viewModel.Id))
                    {
                        return NotFound("Modelo não encontrado!");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Contato/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            
            var viewModel = _mapper.Map<ContatoViewModel>(await _context.Contato
                .FirstOrDefaultAsync(m => m.Id == id));
            if (viewModel == null)
            {
                return NotFound("Modelo não encontrado!");
            }

            return View(viewModel);
        }

        // POST: Contato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var contato = await _context.Contato.FindAsync(id);
            if (contato != null)
            {
                _context.Contato.Remove(contato);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContatoExists(Guid id)
        {
            return _context.Contato.Any(e => e.Id == id);
        }
    }
}
