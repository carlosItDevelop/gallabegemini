using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace VelzonModerna.Controllers
{
    public class ConsolidadoVendedorController : Controller
    {
        private readonly IConsolidadoVendedorRepository _consolidadoVendedorRepository;
        private readonly IConsolidadoVendedorRepositoryDomain _consolidadoVendedorRepositoryDomain;


        public ConsolidadoVendedorController(
            IConsolidadoVendedorRepository consolidadoVendedorRepository, 
            IConsolidadoVendedorRepositoryDomain consolidadoVendedorRepositoryDomain
        )
        {
            _consolidadoVendedorRepository = consolidadoVendedorRepository;
            _consolidadoVendedorRepositoryDomain = consolidadoVendedorRepositoryDomain;
        }

        // Método GET para mostrar a view onde o vendedor será selecionado
        [HttpGet]
        public async Task<IActionResult> SelecionarVendedor()
        {
            try
            {
                // Obter todos os vendedores do repositório
                var vendedores = await _consolidadoVendedorRepositoryDomain.GetAllVendedoresAsync();

                return View(vendedores); // Passa a lista de vendedores para a view
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Erro no servidor ao carregar a lista de vendedores.");
            }
        }

        // Método GET para obter o consolidado de um vendedor específico
        [HttpGet]
        public async Task<IActionResult> ObterConsolidado(Guid vendedorId)
        {
            try
            {
                // Buscar o consolidado de um vendedor específico
                var consolidadoVendedor = await _consolidadoVendedorRepository.ObterConsolidadoVendedorPorIdAsync(vendedorId);

                if (consolidadoVendedor == null)
                {
                    return NotFound("Vendedor não encontrado.");
                }

                return View("ConsolidadoResult", consolidadoVendedor); // Renderiza a View com o consolidado do vendedor
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Erro no servidor ao carregar os dados consolidados.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterItensVenda(Guid vendaId)
        {
            var itensVenda = await _consolidadoVendedorRepository.ObterItensVenda(vendaId);
            if (itensVenda == null || !itensVenda.Itens.Any())
            {
                return NotFound();
            }


            return PartialView("_ItensVendaPartial", itensVenda);  // Supondo que use um PartialView
        }
    }
}
