using System.Security.Claims;
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.Identidade.Data;
using GeneralLabSolutions.Identidade.Services;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.Identidade.Services
{

    public interface IRoleService
    {

        Task<ApplicationUser?> GetUserById(string userId);
        Task AdicionarClaim(ApplicationUser user, string tipo, string valor);
        Task<IdentityResult> CriarRoleAsync(string nomeRole);
        Task<IEnumerable<IdentityRole>> ObterTodasRolesAsync();
        Task<IList<string>> ObterRolesDoUsuarioAsync(string userId);
        Task<IEnumerable<ApplicationUser>> ObterUsuariosPorRoleAsync(string roleName);
        Task<IdentityResult> ExcluirRoleAsync(string roleName);
        UserDto MapearParaUserDto(ApplicationUser user);

    }



    /// <summary>
    /// 
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAspNetUser _aspNetUser;

        public RoleService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAspNetUser aspNetUser)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _aspNetUser = aspNetUser;
        }

        #region: Obter um Usuário por Id

        public async Task<ApplicationUser?> GetUserById(string userId)
        {
            return await Task.FromResult(_userManager.Users.FirstOrDefault(u => u.Id == userId));
        }
        #endregion

        #region: Adicionar Claim

        public async Task AdicionarClaim(ApplicationUser user, string tipo, string valor)
        {
            await _userManager.AddClaimAsync(user, new Claim(tipo, valor));
        }
        #endregion


        #region: Criar Role

        public async Task<IdentityResult> CriarRoleAsync(string nomeRole)
        {
            // Valida se a role já existe
            if (await _roleManager.RoleExistsAsync(nomeRole))
                return IdentityResult.Failed(new IdentityError { Description = "Role já existe." });

            // Cria a role
            var resultado = await _roleManager.CreateAsync(new IdentityRole(nomeRole));
            return resultado;
        }
        #endregion


        #region: Obter todas as Roles Cadastradas

        public async Task<IEnumerable<IdentityRole>> ObterTodasRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
        #endregion


        #region: Obtem Roles de um Usuário
        public async Task<IList<string>> ObterRolesDoUsuarioAsync(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return new List<string>(); // Retorna lista vazia se usuário não for encontrado

            return await _userManager.GetRolesAsync(usuario);
        }
        #endregion


        #region: Obter Usuário por Role

        public async Task<IEnumerable<ApplicationUser>> ObterUsuariosPorRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }
        #endregion


        #region: Excluir Roles

        public async Task<IdentityResult> ExcluirRoleAsync(string roleName)
        {
            // 1. Verificar se a role existe
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role não encontrada." });

            // 2. Verificar se existem usuários associados à role (opcional, mas recomendado)
            var usuariosNaRole = await _userManager.GetUsersInRoleAsync(roleName);
            if (usuariosNaRole.Any())
                return IdentityResult.Failed(new IdentityError { Description = "Não é possível excluir a role pois existem usuários associados a ela." });

            // 3. Excluir a role
            var resultado = await _roleManager.DeleteAsync(role);
            return resultado;
        }

    #endregion


    #region: mapeamento de IdentityUser para UserDto

    /// <summary>
    /// Mapea um IdentityUser para UserDto para evitar 
    /// expor os dados sensíveis da tabela IdentityUser
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public UserDto MapearParaUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                UserId = user.Id,
                NomeCompleto = user.NomeCompleto,
                Apelido = user.Apelido,
                DataNascimento = user.DataNascimento,
                ImgProfilePath = user.ImgProfilePath,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmado = user.EmailConfirmed ? "Sim" : "Não",
                UsuarioBloqueado = user.LockoutEnd != null ? "Sim" : "Não",
                NumeroDeErroDeLogin = user.AccessFailedCount
            };
        }

        #endregion

    }
}
