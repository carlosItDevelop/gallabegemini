using System.Security.Claims;
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using GeneralLabSolutions.Identidade.Services;
using GeneralLabSolutions.WebApiCore.Controllers;
using GeneralLabSolutions.WebApiCore.Identidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace GeneralLabSolutions.Identidade.Controllers
{
    /// <summary>
    /// Controladora que expôe os endpoints de UserAdmin
    /// </summary>
    [Authorize]
    [Route("api/admin")]
    public class UserAdminController : MainController
    {
        private readonly IUserService _identityService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Contrutor da Classe, que injeta IUserService
        /// </summary>
        /// <param name="identityService">Interface do serviço de identidade: "IUserService"</param>
        public UserAdminController(IUserService identityService, IWebHostEnvironment webHostEnvironment)
        {
            _identityService = identityService;
            _webHostEnvironment = webHostEnvironment;
        }


        #region: Retorna uma lista com os todos os usuários cadastrados.

        /// <summary>
        /// Retorna uma lista com os todos os usuários cadastrados.
        /// </summary>
        /// <returns>Retorna uma lista com os todos os usuários cadastrados.</returns>
        /// <remarks>Retorna uma lista com os todos os usuários cadastrados.</remarks>
        [AllowAnonymous]
        [HttpGet("usuarios")]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers()
        {
            var userResponse = await _identityService.GetUsersDto();
            if (userResponse == null)
                return CustomResponse("Usuários não encontrados");

            return Ok(userResponse);
        }

        #endregion


        #region: Endpoint para criação de Usuário Default.

        /// <summary>
        /// Endpoint para criar um novo Usuário
        /// com senha forte e com a Role "Default" atribuída!
        /// </summary>
        /// <param name="criarUsuarioDto">DTO com as propriedades para se criar um novo usuário</param>
        /// <returns>Endpoint para criação de Usuário Default.</returns>
        /// <remarks>Endpoint para criação de Usuário Default.</remarks>
        [ClaimsAuthorize("Usuario", "Criar", "Admin")]
        [HttpPost("criar-usuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarUsuarioAsync(CriarUsuarioDto criarUsuarioDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _identityService.CriarUsuarioAsync(criarUsuarioDto);
            if (resultado.Succeeded)
                return CustomResponse();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Endpoint para retornar um Usuário através da busca por seu Id

        /// <summary>
        /// Endpoint para retornar um Usuário através da busca por seu Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Esperamos um 200 (Ok), um 404 (Not Found), ou um 400(Badrequest)</returns>
        /// <remarks>Endpoint para retornar um Usuário através da busca por seu Id</remarks>
        [ClaimsAuthorize("Usuario", "Consultar", "Admin")]
        [HttpGet("usuarios/{userId}")] // Rota para buscar por ID
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {

            if (!Guid.TryParse(userId, out var userIdGuid))
            {

                AdicionarErroProcessamento("ID de usuário inválido!");
                return CustomResponse();

            }

            var user = await _identityService.GetUserById(userId);

            if (user == null)
            {
                AdicionarErroProcessamento("Usuário não encontrado");
                return CustomResponse();
            }
                
            // Mapear o IdentityUser para um DTO (opcional, mas recomendado)
            var userDto = _identityService.MapearParaUserDto(user);      

            return CustomResponse(userDto);

        }

        #endregion


        #region: Endpoint para adicionar um usuário a uma role existente.

        /// <summary>
        /// Endpoint para adicionar um usuário a uma role existente.
        /// </summary>
        /// <param name="adicionarUsuarioRoleDto">DTO contendo o ID do usuário e o nome da role.</param>
        /// <returns>Retorna um 200 (OK) em caso de sucesso ou um 400 (BadRequest).</returns>
        /// <remarks>Endpoint para adicionar um usuário a uma role existente.</remarks>
        [ClaimsAuthorize("Role", "Atribuir", "Admin")]
        [HttpPost("adicionar-usuario-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarUsuarioRoleAsync(AdicionarUsuarioRoleDto adicionarUsuarioRoleDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _identityService.AdicionarUsuarioARoleAsync(adicionarUsuarioRoleDto.UserId, adicionarUsuarioRoleDto.RoleName);

            if (resultado.Succeeded)
                return CustomResponse();

            // Tratar erros do IdentityResult
            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion

        #region: Lista todas as claims existentes no sistema, com opção de filtragem.

        /// <summary>
        /// Lista todas as claims existentes no sistema, com opção de filtragem.
        /// </summary>
        /// <param name="tipo">Tipo da claim (opcional).</param>
        /// <param name="valor">Valor da claim (opcional).</param>
        /// <returns>Lista de claims.</returns>
        /// <remarks>LISTA DE TODAS AS CLAIMS DO SISTEMA, COM OPÇÃO DE FILTRAGEM.</remarks>
        [ClaimsAuthorize("Claim", "Listar", "Admin")]
        [HttpGet("claims")]
        [ProducesResponseType(typeof(IEnumerable<Claim>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarClaimsAsync(string? tipo = null, string? valor = null)
        {
            var claims = await _identityService.ObterTodasClaimsAsync(tipo, valor);
            return CustomResponse(claims);
        }

        #endregion


        #region: Lista as claims associadas a um usuário, com opção de filtragem.

        /// <summary>
        /// Lista as claims associadas a um usuário, com opção de filtragem.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="tipo">Tipo da claim (opcional).</param>
        /// <param name="valor">Valor da claim (opcional).</param>
        /// <returns>Lista de claims do usuário.</returns>
        [ClaimsAuthorize("Usuario", "ConsultarClaims", "Admin")]
        [HttpGet("usuarios/{userId}/claims")]
        [ProducesResponseType(typeof(IEnumerable<Claim>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarClaimsDoUsuarioAsync(string userId, string? tipo = null, string? valor = null)
        {

            if (string.IsNullOrEmpty(userId))
            {
                AdicionarErroProcessamento("O Id do Usuário é obrigatório!");
                return CustomResponse();
            }

            var claims = await _identityService.ObterClaimsDoUsuarioAsync(userId, tipo, valor);
            return CustomResponse(claims);
        }

        #endregion


        #region: Lista os usuários que possuem uma claim específica.

        /// <summary>
        /// Lista os usuários que possuem uma claim específica.
        /// </summary>
        /// <param name="tipo">Tipo da claim.</param>
        /// <param name="valor">Valor da claim.</param>
        /// <returns>Lista de usuários com a claim.</returns>
        /// <remarks>LISTA DE USUÁRIOS COM A CLAIM.</remarks>
        [ClaimsAuthorize("Claim", "ConsultarUsuarios", "Admin")]
        [HttpGet("claims/usuarios")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListarUsuariosPorClaimAsync([FromQuery] string tipo, [FromQuery] string valor)
        {
            if(string.IsNullOrEmpty(tipo) || string.IsNullOrEmpty(valor)){
                AdicionarErroProcessamento("Type e Value são campos obrigatórios!");
                return CustomResponse();
            }

            var usuarios = await _identityService.ObterUsuariosPorClaimAsync(tipo, valor);

            /*
             Por questão de segurança, não posso expor todos os ados dos usuários!
             */
            var usuariosDto = usuarios.Select(u => _identityService.MapearParaUserDto(u)).ToList();

            return CustomResponse(usuariosDto);
        }

        #endregion


        #region: Excluir uma Claim de um Usuário

        /// <summary>
        /// Exclui uma claim de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="claimType">Tipo da claim.</param>
        /// <param name="claimValue">Valor da claim.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Claim", "Excluir", "Admin")]
        [HttpDelete("usuarios/{userId}/claims")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExcluirClaimDoUsuarioAsync(string userId, string claimType, string claimValue)
        {
            var resultado = await _identityService.ExcluirClaimDoUsuarioAsync(userId, claimType, claimValue);
            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Atualiza os dados de um Usuário

        /// <summary>
        /// Atualiza os dados de um usuário.
        /// </summary>
        /// <param name="dto">DTO com os dados do usuário a serem atualizados.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Usuario", "Atualizar", "Admin")]
        [HttpPut("usuarios/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarUsuarioAsync(AtualizarUsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _identityService.AtualizarUsuarioPorAdminAsync(dto);

            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Atualiza a senha de Um Usuário

        /// <summary>
        /// Atualiza a senha de um usuário.
        /// </summary>
        /// <param name="dto">DTO com os dados para atualização da senha.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [Authorize] // Qualquer usuário autenticado pode atualizar sua própria senha
        [HttpPut("usuarios/senha")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarSenhaAsync(AtualizarSenhaDto dto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _identityService.AtualizarSenhaAsync(dto);

            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Ativa ou inativa um usuário.

        /// <summary>
        /// Ativa ou inativa um usuário.
        /// ATENÇÃO: Como o parâmetro é `Ativar`, 
        /// passar "true" para Ativar e "false" para Desativar;
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="ativar">Indica se o usuário deve ser ativado (true) ou inativado (false).</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Usuario", "AtivarInativar", "Admin")]
        [HttpPut("usuarios/{userId}/ativar-inativar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtivarDesativarUsuarioAsync(string userId, [FromQuery] bool ativar)
        {
            var resultado = await _identityService.AtivarDesativarUsuarioAsync(userId, ativar);

            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Excluir um Usuário

        /// <summary>
        /// Exclui um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Usuario", "Excluir", "Admin")]
        [HttpDelete("usuarios/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExcluirUsuarioAsync(string userId)
        {
            var resultado = await _identityService.ExcluirUsuarioAsync(userId);

            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Bloquear um Usuário (NÃO Admin) por 'X' minutos

        /// <summary>
        /// Bloqueia ou desbloqueia um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="minutosBloqueio">Tempo de bloqueio em minutos (opcional). 
        /// Se não for informado, o usuário será desbloqueado.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Usuario", "BloquearDesbloquear", "Admin")]
        [HttpPut("usuarios/{userId}/bloquear-desbloquear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BloquearDesbloquearUsuarioAsync(string userId, int? minutosBloqueio = null)
        {
            var tempoBloqueio = minutosBloqueio.HasValue ? TimeSpan.FromMinutes(minutosBloqueio.Value) : (TimeSpan?)null;

            var resultado = await _identityService.BloquearDesbloquearUsuarioAsync(userId, tempoBloqueio);

            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion


        #region: Endpoint para Upload de imagem de perfil de Usuário.

        /// <summary>
        /// Realiza o upload da imagem de perfil de um usuário.
        /// </summary>
        /// <param name="dto">DTO contendo o ID do usuário e a imagem.</param>
        /// <returns>OK (200) com o caminho da imagem em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Usuario", "UploadImagem", "Admin")]
        [HttpPost("usuarios/imagem")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImagemAsync([FromForm] UploadImagemDto dto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            // 1. Chamar o método UploadImagemAsync do UserService
            var resultado = await _identityService.UploadImagemAsync(dto);

            if (resultado.Succeeded) // Comparar com resultado.Succeeded (booleano)
            {
                var usuario = await _identityService.GetUserById(dto.UserId);
                return CustomResponse(usuario!.ImgProfilePath);
            }

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();

        }

        #endregion

        #region: Obtem a ImgProfilePath

        /// <summary>
        /// Retorna a ImgProfilePath
        /// </summary>
        /// <param name="nomeImagem"></param>
        /// <returns>Retorna a ImgProfilePath</returns>
        /// <remarks>Retorna a ImgProfilePath</remarks>
        [AllowAnonymous]
        [HttpGet("obter-imagem/{nomeImagem}")]
        public async Task<IActionResult> ObterImagem(string nomeImagem)
        {
            // Obter o caminho físico da imagem dentro da pasta wwwroot
            var caminhoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles", nomeImagem);

            // Verificar se a imagem existe
            if (!System.IO.File.Exists(caminhoImagem))
            {
                return NotFound(); // Retornar NotFound se a imagem não for encontrada
            }

            // Obter o tipo MIME da imagem (ex: image/png)
            new FileExtensionContentTypeProvider().TryGetContentType(nomeImagem, out var contentType);

            // Ler o conteúdo da imagem como um array de bytes
            var bytesImagem = await System.IO.File.ReadAllBytesAsync(caminhoImagem);

            // Retornar a imagem como um FileResult
            return File(bytesImagem, contentType ?? "application/octet-stream"); // Tipo MIME padrão se não for possível determinar
        }

        #endregion


        #region: Remover um usuário de uma role específica


        [ClaimsAuthorize("Role", "Remover", "Admin")] // Ou a claim apropriada
        [HttpDelete("usuarios/{userId}/roles/{roleName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverUsuarioDeRoleAsync(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                AdicionarErroProcessamento("UserId e RoleName são obrigatórios.");
                return CustomResponse();
            }

            var resultado = await _identityService.RemoverUsuarioDeRoleAsync(userId, roleName); // CHAMA O NOVO MÉTODO DO SERVIÇO

            if (resultado.Succeeded)
            {
                return NoContent();
            }

            AdicionarErrosIdentityResult(resultado); // Seu método auxiliar
            return CustomResponse();
        }

        #endregion


        // Deixar este método por último

        #region: Método auxiliar para tratar erros do IdentityResult
        private void AdicionarErrosIdentityResult(IdentityResult resultado)
        {
            foreach (var erro in resultado.Errors)
            {
                AdicionarErroProcessamento(erro.Description);
            }
        }
        #endregion

    }

}
