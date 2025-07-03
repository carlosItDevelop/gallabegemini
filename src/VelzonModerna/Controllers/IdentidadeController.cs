using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;
using VelzonModerna.Services;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Controllers
{
    [Route("identidade")]
    /// <summary>
    /// Controller para gerenciar a autenticação de usuários
    /// </summary>
    public class IdentidadeController : BaseMvcToApiController
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public IdentidadeController(
            IAutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpGet]
        [Route("perfil")]
        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        [Route("acesso-negado")]
        public IActionResult AcessoNegado()
        {
            return View();
        }

        [HttpGet]
        [Route("autenticar")]
        public IActionResult Registro()
        {
            return View();
        }
        

        [HttpPost]
        [Route("autenticar")]
        public async Task<IActionResult> Registro(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
                return View(usuarioRegistro);

            var resposta = await _autenticacaoService.Registro(usuarioRegistro);

            if (ResponsePossuiErros(resposta.ResponseResult))
                return View(usuarioRegistro);

            await _autenticacaoService.RealizarLogin(resposta);

            return RedirectToAction("GlDashboard", "GalLabs");
        }

        [HttpGet]
        [Route("logar")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData ["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("logar")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            ViewData ["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(usuarioLogin);

            var resposta = await _autenticacaoService.Login(usuarioLogin);

            if (ResponsePossuiErros(resposta.ResponseResult))
                return View(usuarioLogin);

            await _autenticacaoService.RealizarLogin(resposta);

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("GlDashboard", "GalLabs");

            return LocalRedirect(returnUrl);

        }

        [HttpGet]
        [Route("sair")]
        public async Task<IActionResult> Logout()
        {
            await _autenticacaoService.Logout();
            return RedirectToAction("Login", "Identidade");
        }
    }

}

