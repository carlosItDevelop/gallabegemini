// No projeto MVC: VelzonModerna/Services/IUserAdminMvcService.cs
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade; // Para DTOs e ResponseResult
using Microsoft.AspNetCore.Http; // Para IFormFile
using System.Collections.Generic;
using System.Threading.Tasks;
using VelzonModerna.ViewModels;

namespace VelzonModerna.Services
{
    public interface IUserAdminMvcService
    {
        Task<IEnumerable<UserResponseDto>> ObterTodosUsuariosAsync();
        Task<UserDto> ObterUsuarioPorIdAsync(string userId); // Assumindo que UserDto estará em CoreShared
        Task<ResponseResult> CriarUsuarioAsync(CriarUsuarioDto dto); // Assumindo CriarUsuarioDto em CoreShared
        Task<ResponseResult> AdicionarUsuarioRoleAsync(AdicionarUsuarioRoleDto dto); // Assumindo AdicionarUsuarioRoleDto em CoreShared
        Task<IEnumerable<UsuarioClaim>> ObterTodasClaimsSistemaAsync(string? tipo = null, string? valor = null);
        Task<IEnumerable<UsuarioClaim>> ObterClaimsDoUsuarioAsync(string userId, string? tipo = null, string? valor = null);
        Task<IEnumerable<UserDto>> ObterUsuariosPorClaimAsync(string tipo, string valor); // Assumindo UserDto em CoreShared
        Task<ResponseResult> ExcluirClaimDoUsuarioAsync(string userId, string claimType, string claimValue);
        Task<ResponseResult> AtualizarUsuarioAsync(string userId, AtualizarUsuarioDto dto); // Assumindo ActualizarUsuarioDto em CoreShared (verifique nome)
        Task<ResponseResult> AtualizarSenhaAdminAsync(AtualizarSenhaDto dto); // Assumindo AtualizarSenhaDto em CoreShared
        Task<ResponseResult> AtivarDesativarUsuarioAsync(string userId, bool ativar);
        Task<ResponseResult> ExcluirUsuarioAsync(string userId);
        Task<ResponseResult> BloquearDesbloquearUsuarioAsync(string userId, int? minutosBloqueio = null);
        Task<(ResponseResult Response, string ImagemPath)> UploadImagemPerfilAsync(string userId, IFormFile imagem); // O DTO da API (UploadImagemDto) não é usado diretamente na chamada do serviço MVC.

        string GetUserAdminApiBaseUrl(); // Novo método


        // --- NOVOS MÉTODOS PARA ROLES ---
        Task<IEnumerable<RoleViewModel>> ObterTodasRolesAsync();
        Task<IList<string>> ObterRolesDoUsuarioAsync(string userId); // Para listar as roles que um usuário já possui
        // Task<ResponseResult> AdicionarUsuarioRoleAsync(AdicionarUsuarioRoleDto dto); // Já existe
        Task<ResponseResult> RemoverUsuarioDeRoleAsync(string userId, string roleName); // Para remover role


    }
}