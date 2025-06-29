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

            await _vendedorDomainService.AtualizarVendedor(vendedorCompleto);

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

            if (vendedor is null) BadRequest("Vendedor não encontrado.");

            await _vendedorDomainService.ExcluirVendedor(id);

            if (!OperacaoValida())
            {
                var vnedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);
                return View(vnedorViewModel); // Retorna para view com o vendedor para exibir erros de validação.
            }

            if (!await _vendedorRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir os dados.");

                var vendedorViewModel = _mapper.Map<VendedorViewModel>(vendedor);

                return View(nameof(Delete), vendedorViewModel);
            }

            TempData ["Success"] = "Vendedor Excluído com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

    }
}
