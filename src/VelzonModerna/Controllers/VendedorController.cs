using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    public class VendedorController : Controller
    {
        private readonly IVendedorDomainService _vendedorDomainService;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IMapper _mapper;
        private readonly INotificador _notificador;

        public VendedorController(IVendedorDomainService vendedorDomainService, IVendedorRepository vendedorRepository, IMapper mapper, INotificador notificador)
        {
            _vendedorDomainService = vendedorDomainService;
            _vendedorRepository = vendedorRepository;
            _mapper = mapper;
            _notificador = notificador;
        }

        // GET: Vendedor
        public async Task<IActionResult> Index()
        {
            var vendedores = await _vendedorDomainService.ObterTodosVendedores();
            return View(_mapper.Map<IEnumerable<VendedorViewModel>>(vendedores));
        }

        // GET: Vendedor/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _vendedorDomainService.ObterVendedorPorId(id.Value);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<VendedorViewModel>(vendedor));
        }

        // GET: Vendedor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendedorViewModel vendedorViewModel)
        {
            if (!ModelState.IsValid) return View(vendedorViewModel);

            var vendedor = _mapper.Map<Vendedor>(vendedorViewModel);
            await _vendedorDomainService.AdicionarVendedor(vendedor);

            if (!OperacaoValida())
            {
                return View(vendedorViewModel);
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados.");
                return View(vendedorViewModel);
            }

            TempData["Success"] = "Vendedor Adicionado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendedor/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _vendedorDomainService.ObterVendedorPorId(id.Value);
            if (vendedor == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<VendedorViewModel>(vendedor));
        }

        // POST: Vendedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VendedorViewModel vendedorViewModel)
        {
            if (id != vendedorViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(vendedorViewModel);

            var vendedor = _mapper.Map<Vendedor>(vendedorViewModel);
            await _vendedorDomainService.AtualizarVendedor(vendedor);

            if (!OperacaoValida())
            {
                return View(vendedorViewModel);
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados.");
                return View(vendedorViewModel);
            }

            TempData["Success"] = "Vendedor Atualizado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendedor/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _vendedorDomainService.ObterVendedorPorId(id.Value);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<VendedorViewModel>(vendedor));
        }

        // POST: Vendedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _vendedorDomainService.ExcluirVendedor(id);

            if (!OperacaoValida())
            {
                return View(_mapper.Map<VendedorViewModel>(await _vendedorDomainService.ObterVendedorPorId(id)));
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir os dados.");
                return View(_mapper.Map<VendedorViewModel>(await _vendedorDomainService.ObterVendedorPorId(id)));
            }

            TempData["Success"] = "Vendedor Excluído com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        protected bool OperacaoValida()
        {
            if (!_notificador.TemNotificacao())
            {
                return true;
            }

            _notificador.ObterNotificacoes().ForEach(n => ModelState.AddModelError(string.Empty, n.Mensagem));
            return false;
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
