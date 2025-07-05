using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace VelzonModerna.Controllers
{
    [Route("pedidos")]
    public class PedidoController : Controller
    {
        // Todo: Implementar repository Pattern
        private readonly AppDbContext _context;
        //private readonly IPedidoRepository _pedidoRepository;

        //public PedidoController(IPedidoRepository pedidoRepository)
        //{
        //    _pedidoRepository = pedidoRepository;
        //}

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        private void PopulateDropdowns(Guid? selectedClienteId = null, Guid? selectedVendedorId = null, StatusDoPedido? selectedStatus = null)
        {
            ViewBag.ClienteId = new SelectList(_context.Cliente, "Id", "Nome", selectedClienteId);
            ViewBag.VendedorId = new SelectList(_context.Vendedor, "Id", "Nome", selectedVendedorId);
            ViewBag.StatusDoPedido = new SelectList(Enum.GetValues(typeof(StatusDoPedido)).Cast<StatusDoPedido>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", selectedStatus);
        }



        [HttpGet]
        [Route("lista-de-pedidos")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Vendedor)
                .Include(p => p.Itens)
            .AsNoTracking()
            .ToListAsync();

            //var appDbContext = await _pedidoRepository.GetAllPedidosWithIncludesAsync();

            //var resumo = await _pedidoRepository.GetQuantidadeEValorTotalPorStatusAsync(StatusDoPedido.Orcamento);
            //Console.WriteLine($"Quantidade: {resumo.Quantidade}, Valor Total: {resumo.ValorTotal}");


            //return View(appDbContext);
            return View(await appDbContext);
        }

        [Route("detalhe-do-pedido/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {

            var pedido = await _context.Pedido
                .Include(p => p.Itens)
                    .ThenInclude(p=>p.Produto)
                        .ThenInclude(f=>f.Fornecedor)
                .Include(p => p.Cliente)
                .Include(p => p.Vendedor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);


            if (pedido == null)
            {
                return NotFound("Pedido não encontrado!");
            }

            //var total = pedido.Itens.Sum(i => i.ValorUnitario * i.Quantidade);
            //ViewBag.TotalDoPedido = total;

            return View(pedido);

        }

        [HttpGet("adicionar-pedido")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }


        [HttpPost("adicionar-pedido")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(pedido.ClienteId, pedido.VendedorId, pedido.StatusDoPedido);
            return View(pedido);
        }

        [Route("atualizar-pedido/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            PopulateDropdowns(pedido.ClienteId, pedido.VendedorId, pedido.StatusDoPedido);
            return View(pedido);
        }


        [HttpPost]
        [Route("atualizar-pedido/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    } else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(pedido.ClienteId, pedido.VendedorId, pedido.StatusDoPedido);
            return View(pedido);
        }

        [Route("excluir-pedido/{id:guid}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        [Route("excluir-pedido/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(Guid id)
        {
            return _context.Pedido.Any(e => e.Id == id);
        }
    }
}
