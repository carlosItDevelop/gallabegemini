using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using GeneralLabSolutions.Identidade.Data;
using GeneralLabSolutions.Identidade.Services;
using GeneralLabSolutions.WebApiCore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GeneralLabSolutions.Identidade.Controllers
{

    [Route("api/identidade")]
    public class AuthController : MainController
    {

        private readonly AuthenticationIdentityService _authenticationService;

        public AuthController(AuthenticationIdentityService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        /// <summary>
        /// Endpoint para registro de Usuários
        /// </summary>
        /// <param name="usuarioRegistro"></param>
        /// <returns>Retorna um 201, Created</returns>
        /// <remarks>Endpoint para registro de Usuários</remarks>
        [HttpPost("autenticar")]
        [ProducesResponseType(typeof(UsuarioRegistro), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var user = new ApplicationUser
            {
                Apelido = usuarioRegistro.Apelido,
                DataNascimento = usuarioRegistro.DataNascimento,
                NomeCompleto = usuarioRegistro.NomeCompleto,
                ImgProfilePath = usuarioRegistro.ImgProfilePath,
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _authenticationService._userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.GerarJwt(usuarioRegistro.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }


        /// <summary>
        /// Endpoint de Login
        /// </summary>
        /// <param name="usuarioLogin"></param>
        /// <returns>Retorna um UsuarioLogin</returns>
        /// <remarks>Digite seu email e senha para fazer login no sistema</remarks>
        [HttpPost("logar")]
        [ProducesResponseType(typeof(UsuarioLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await _authenticationService._signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha,
                false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.GerarJwt(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");
            return CustomResponse();

        }


        /// <summary>
        /// Retorna um Token e um Refresh Token para Atualizar o Token de Acesso.
        /// </summary>
        /// <param name="refreshToken">Informe o Refres Token</param>
        /// <returns>Retorna um Token e um Refresh Token para Atualizar o Token de Acesso.</returns>
        /// <remarks>Retorna um Token e um Refresh Token para Atualizar o Token de Acesso.</remarks>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshToken), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AdicionarErroProcessamento("Refresh Token Inválido!");
                return CustomResponse();
            }

            var token = await _authenticationService.ObterRefreshToken(Guid.Parse(refreshToken));

            if (token is null)
            {
                AdicionarErroProcessamento("Refresh Token Expirado!");
                return CustomResponse();
            }

            return CustomResponse(await _authenticationService.GerarJwt(token!.Username));


        }


        /// <summary>
        /// Renova o token de acesso utilizando um refresh token.
        /// </summary>
        /// <param name="dto">DTO contendo o refresh token.</param>
        /// <returns>Um novo access token e refresh token em caso de sucesso. BadRequest em caso de erro.</returns>
        [HttpPost("renovar-token")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RenovarTokenAsync([FromBody] RenovarTokenDto dto)
        {
            // 1. Validar o refresh token
            if (string.IsNullOrEmpty(dto.RefreshToken))
            {
                AdicionarErroProcessamento("Refresh token é obrigatório.");
                return CustomResponse();
            }

            // 2. Buscar o refresh token (Corrigido)
            var refreshToken = await _authenticationService.ObterRefreshToken(Guid.Parse(dto.RefreshToken));

            if (refreshToken is null)
            {
                AdicionarErroProcessamento("Refresh token inválido ou expirado.");
                return CustomResponse();
            }

            // 3. Buscar o usuário associado ao refresh token (Corrigido)
            var usuario = await _authenticationService._userManager.FindByNameAsync(refreshToken.Username);

            if (usuario is null)
            {
                AdicionarErroProcessamento("Usuário não encontrado.");
                return CustomResponse();
            }

            // 4. Gerar novo access token e refresh token 
            var novoAccessToken = await _authenticationService.GerarJwt(usuario.Email);

            // 5. Retornar os tokens
            return CustomResponse(new
            {
                AccessToken = novoAccessToken.AccessToken,
                RefreshToken = novoAccessToken.RefreshToken
            });
        }


    }
}
