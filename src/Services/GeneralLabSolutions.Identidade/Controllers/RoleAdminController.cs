using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.Identidade.Services;
using GeneralLabSolutions.WebApiCore.Controllers;
using GeneralLabSolutions.WebApiCore.Identidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeneralLabSolutions.Identidade.Controllers
{
    /// <summary>
    /// Controladora que expôe os endpoints de UserAdmin
    /// </summary>
    [Authorize]
    [Route("api/role")]
    public class RoleAdminController : MainController
    {
        private readonly IRoleService _identityService;

        public RoleAdminController(IRoleService identityService)
        {
            _identityService = identityService;
        }


        #region: Endpoint para adicionar uma Claim a um Usuário.

        /// <summary>
        /// Endpoint para adicionar uma Claim a um Usuário.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>Endpoint para adicionar uma Claim a um Usuário.</returns>
        /// <remarks>ENDPOINT PARA ADICIONAR UMA CLAIM A UM USUÁRIO.</remarks>
        [ClaimsAuthorize("Claim", "Adicionar", "Admin")]
        [HttpPost("adicionar-claim")]
        public async Task<IActionResult> AdicionarClaim(string userId, string type, string value)
        {

            if (string.IsNullOrEmpty(userId)
                || string.IsNullOrEmpty(type)
                    || string.IsNullOrEmpty(value))
            {
                AdicionarErroProcessamento("Usuário, Tipo e Valor são dados obrigatórios!");
                return CustomResponse();
            }

            var user = await _identityService.GetUserById(userId);
            if (user == null)
            {
                AdicionarErroProcessamento("Usuário é um dado obrigatório!");
                return CustomResponse();
            }


            await _identityService.AdicionarClaim(user, type, value);
            return CustomResponse();
        }

        #endregion

        #region: Endpoint para criação de Roles.

        /// <summary>
        /// Endpoint para criar uma nova role.
        /// </summary>
        /// <param name="criarRoleDto">DTO contendo o nome da role.</param>
        /// <returns>Retorna um 201 (Created) em caso de sucesso ou um 400 (BadRequest).</returns>
        /// <remarks>Endpoint para criação de Roles.</remarks>
        [ClaimsAuthorize("Role", "Criar", "Admin")]
        [HttpPost("criar-role")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarRoleAsync(CriarRoleDto criarRoleDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _identityService.CriarRoleAsync(criarRoleDto.Nome);
            if (resultado.Succeeded)
                return CustomResponse();

            // Tratar erros do IdentityResult
            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion

        #region: Lista todas as roles existentes no sistema.

        /// <summary>
        /// Lista todas as roles existentes no sistema.
        /// </summary>
        /// <returns>Lista de roles.</returns>
        /// <remarks>LISTA TODAS AS ROLES EXISTENTES NO SISTEMA.</remarks>
        [ClaimsAuthorize("Role", "Listar", "Admin")]
        [HttpGet("roles")]
        [ProducesResponseType(typeof(IEnumerable<IdentityRole>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarRolesAsync()
        {
            var roles = await _identityService.ObterTodasRolesAsync();
            return CustomResponse(roles);
        }

        #endregion

        #region: Lista de roles do usuário.

        /// <summary>
        /// Lista as roles associadas a um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>Lista de roles do usuário.</returns>
        /// <remarks>LISTA DE ROLES DO USUÁRIO.</remarks>
        [ClaimsAuthorize("Usuario", "ConsultarRoles", "Admin")]
        [HttpGet("usuarios/{userId}/roles")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarRolesDoUsuarioAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                AdicionarErroProcessamento("O Id do Usuário é obrigatório!");
                return CustomResponse();
            }

            var roles = await _identityService.ObterRolesDoUsuarioAsync(userId);
            return CustomResponse(roles);
        }

        #endregion

        #region: Lista os usuários que pertencem a uma determinada role.

        /// <summary>
        /// Lista os usuários que pertencem a uma determinada role.
        /// </summary>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>Lista os usuários, numa Lista de UserDto, que pertencem a uma determinada role.</returns>
        /// <remarks>LISTA OS USUÁRIOS, NUMA LISTA DE USERDTO, QUE PERTENCEM A UMA DETERMINADA ROLE.</remarks>
        [ClaimsAuthorize("Role", "ConsultarUsuarios", "Admin")]
        [HttpGet("roles/{roleName}/usuarios")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListarUsuariosPorRoleAsync(string roleName)
        {

            if (string.IsNullOrEmpty(roleName))
            {
                AdicionarErroProcessamento("É necessário informar uma 'Role'.");
                return CustomResponse();

            }

            var usuarios = await _identityService.ObterUsuariosPorRoleAsync(roleName);

            if (usuarios == null || !usuarios.Any())
            {
                AdicionarErroProcessamento("Nenhum Usuário foi encontrado com esta 'Role'.");
                return CustomResponse(usuarios!);
            }

            /*
             Por questão de segurança, não posso expor todos os dados dos usuários!
             */
            var usuariosDto = usuarios.Select(u => _identityService.MapearParaUserDto(u)).ToList();

            return CustomResponse(usuariosDto);
        }

        #endregion


        #region: Excluir uma Role

        /// <summary>
        /// Exclui uma role.
        /// </summary>
        /// <param name="roleName">Nome da role.</param>
        /// <returns>NoContent (204) em caso de sucesso ou BadRequest (400) em caso de erro.</returns>
        [ClaimsAuthorize("Role", "Excluir", "Admin")]
        [HttpDelete("roles/{roleName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExcluirRoleAsync(string roleName)
        {
            var resultado = await _identityService.ExcluirRoleAsync(roleName);
            if (resultado.Succeeded)
                return NoContent();

            AdicionarErrosIdentityResult(resultado);
            return CustomResponse();
        }

        #endregion

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
