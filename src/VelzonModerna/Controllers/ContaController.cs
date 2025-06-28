using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    public class ContaController : BaseMvcController
    {
        private readonly IMapper _mapper;
        private readonly IContaService _contaService;
        private readonly IContaRepository _contaRepository; //Para o Index e Details
        private readonly IGenericRepository<Conta, Guid> _query;

        public ContaController(INotificador notificador,
                              IMapper mapper,
                              IContaService contaService,
                              IContaRepository contaRepository,
                              IGenericRepository<Conta, Guid> query) : base(notificador)
        {
            _mapper = mapper;
            _contaService = contaService;
            _contaRepository = contaRepository;
            _query = query;
        }

        // GET: Conta
        public async Task<IActionResult> Index()
        {
            var contas = await _query.GetAllAsync();
            var contasViewModel = _mapper.Map<IEnumerable<ContaViewModel>>(contas);
            return View(contasViewModel);
        }

        // GET: Conta/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var contaViewModel = _mapper.Map<ContaViewModel>(await _query.GetByIdAsync(id));

            if (contaViewModel == null)
            {
                return NotFound();
            }
            return View(contaViewModel);
        }

        // GET: Conta/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContaViewModel contaViewModel)
        {
            if (!ModelState.IsValid)
                return View(contaViewModel);

            var conta = _mapper.Map<Conta>(contaViewModel);
            var result = await _contaService.AdicionarConta(conta);

            if (!result)
                return View(contaViewModel); // Exibe notificações

            await _contaRepository.UnitOfWork.CommitAsync(); //Salva no Banco!
            TempData ["Success"] = "Conta adicionada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Conta/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var contaViewModel = _mapper.Map<ContaViewModel>(await _query.GetByIdAsync(id));

            if (contaViewModel == null)
            {
                return NotFound();
            }
            return View(contaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ContaViewModel contaViewModel)
        {
            if (id != contaViewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(contaViewModel);

            var conta = await _query.GetByIdAsync(id);
            if (conta == null)
                return NotFound();

            _mapper.Map(contaViewModel, conta); // Atualiza a entidade

            var result = await _contaService.AtualizarConta(conta);
            if (!result)
                return View(contaViewModel);

            await _contaRepository.UnitOfWork.CommitAsync(); //Persiste!
            TempData ["Success"] = "Conta atualizada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Conta/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var contaViewModel = _mapper.Map<ContaViewModel>(await _query.GetByIdAsync(id));
            if (contaViewModel == null)
                return NotFound();

            return View(contaViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var conta = await _query.GetByIdAsync(id);
            if (conta == null)
                return NotFound();

            var result = await _contaService.RemoverConta(conta);
            if (!result)
            {
                //Se a regra de negocio foi violada, retorna pra View!
                var contaViewModel = _mapper.Map<ContaViewModel>(conta);
                return View("Delete", contaViewModel);
            }

            await _contaRepository.UnitOfWork.CommitAsync(); //Persiste!
            TempData ["Success"] = "Conta excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // Ações para Marcar como Paga/Não Paga
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoPaga(Guid id)
        {
            await _contaService.MarcarComoPaga(id);
            if (!OperacaoValida())
                return RedirectToAction(nameof(Index)); // Fica na mesma página
            await _contaRepository.UnitOfWork.CommitAsync();

            TempData ["Success"] = "Conta marcada como paga com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoNaoPaga(Guid id)
        {
            await _contaService.MarcarComoNaoPaga(id);
            if (!OperacaoValida())
                return RedirectToAction(nameof(Index)); // Fica na mesma página
            await _contaRepository.UnitOfWork.CommitAsync();
            TempData ["Success"] = "Conta estornada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inativar(Guid id)  //Inativar, melhor que Excluir!
        {
            await _contaService.InativarConta(id);
            if (!OperacaoValida())
                return RedirectToAction(nameof(Index)); // Fica na mesma página
            await _contaRepository.UnitOfWork.CommitAsync();
            TempData ["Success"] = "Conta Inativada com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}