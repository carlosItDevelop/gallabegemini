using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Controllers
{
    public class ConsolidadoFornecedorController : Controller
    {
        private readonly IConsolidadoFornecedorRepository _consolidadoFornecedorRepository;
        private readonly IConsolidadoFornecedorRepositoryDomain _consolidadoFornecedorRepositoryDomain;


        public ConsolidadoFornecedorController(IConsolidadoFornecedorRepository consolidadoFornecedorRepository, IConsolidadoFornecedorRepositoryDomain consolidadoFornecedorRepositoryDomain)
        {
            _consolidadoFornecedorRepository = consolidadoFornecedorRepository;
            _consolidadoFornecedorRepositoryDomain = consolidadoFornecedorRepositoryDomain;
        }

        [HttpGet]
        public async Task<IActionResult> SelecionarFornecedor()
        {
            try
            {
                var fornecedores = await _consolidadoFornecedorRepositoryDomain.ObterTodosFornecedoresAsync();
                return View(fornecedores);
            } catch (Exception ex)
            {
                // Logar a exceção
                Console.WriteLine(ex);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterConsolidado(Guid fornecedorId)
        {
            try
            {
                var consolidadoFornecedor = await _consolidadoFornecedorRepository.ObterConsolidadoFornecedorPorIdAsync(fornecedorId);

                if (consolidadoFornecedor == null)
                {
                    return PartialView("_FornecedorSemResumoPartial");
                }

                return View("ConsolidadoResult", consolidadoFornecedor);
            } catch (Exception ex)
            {
                // Logar a exceção
                Console.WriteLine(ex);
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}