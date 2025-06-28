// Em VelzonModerna/Services/IAutenticacaoService.cs
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using System.Threading.Tasks;

namespace VelzonModerna.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);
        Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro);
        Task<UsuarioRespostaLogin> UtilizarRefreshToken(string refreshToken);
        Task RealizarLogin(UsuarioRespostaLogin resposta);
        Task Logout();
        bool TokenExpirado();
        Task<bool> RefreshTokenValido();
        string GetApiBaseUrl(); // NOVO MÉTODO
    }
}