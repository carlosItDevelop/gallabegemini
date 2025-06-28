using System.Security.Claims;
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using GeneralLabSolutions.Identidade.Data;
using Microsoft.AspNetCore.Identity;

namespace GeneralLabSolutions.Identidade.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersDto();
        Task<ApplicationUser?> GetUserById(string userId);
        Task<IdentityResult> CriarUsuarioAsync(CriarUsuarioDto criarUsuarioDto);
        Task<IdentityResult> AdicionarUsuarioARoleAsync(string userId, string roleName);

        Task<IEnumerable<Claim>> ObterTodasClaimsAsync(string? tipo = null, string? valor = null);

        Task<IEnumerable<ApplicationUser>> ObterUsuariosPorClaimAsync(string tipo, string valor);
        Task<IEnumerable<Claim>> ObterClaimsDoUsuarioAsync(string userId, string? tipo = null, string? valor = null);

        UserDto MapearParaUserDto(ApplicationUser user);
        Task<IdentityResult> ExcluirClaimDoUsuarioAsync(string userId, string claimType, string claimValue);
        Task<IdentityResult> ExcluirUsuarioAsync(string userId);
        Task<IdentityResult> AtualizarUsuarioPorAdminAsync(AtualizarUsuarioDto dto);
        Task<IdentityResult> AtualizarSenhaAsync(AtualizarSenhaDto dto);

        Task<IdentityResult> AtivarDesativarUsuarioAsync(string userId, bool ativar);

        Task<IdentityResult> BloquearDesbloquearUsuarioAsync(string userId, TimeSpan? tempoBloqueio = null);

        Task<IdentityResult> UploadImagemAsync(UploadImagemDto dto);

        // Novo: Documentar...
        Task<IdentityResult> RemoverUsuarioDeRoleAsync(string userId, string roleName);

    }
}
