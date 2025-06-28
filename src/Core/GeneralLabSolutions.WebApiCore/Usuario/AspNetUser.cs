using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GeneralLabSolutions.WebApiCore.Usuario
{

    public interface IAspNetUser
    {
        string Name { get; }
        Guid ObterUserId();
        string ObterUserEmail();
        string ObterUserToken();
        string ObterUserRefreshToken();
        bool EstaAutenticado();
        bool PossuiRole(string role);
        IEnumerable<Claim> ObterClaims();
        HttpContext ObterHttpContext();

        // Novas propriedades/métodos
        string ObterNomeCompleto();
        string ObterApelido();
        DateTime? ObterDataNascimento(); // Nullable se a claim puder não existir ou falhar no parse
        string ObterImgProfilePath();

    }



    namespace GeneralLabSolutions.WebApiCore.Usuario
    {
        // Interface IAspNetUser permanece a mesma

        public class AspNetUser : IAspNetUser
        {
            private readonly IHttpContextAccessor _accessor;

            public AspNetUser(IHttpContextAccessor accessor)
            {
                _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor)); // Boa prática adicionar null check no construtor
            }

            // Helper para verificar se o contexto e o usuário são válidos
            private ClaimsPrincipal? GetUser() => _accessor?.HttpContext?.User;

            // Propriedade Name precisa de proteção
            public string Name => GetUser()?.Identity?.Name ?? string.Empty; // Retorna string vazia se algo for nulo

            public Guid ObterUserId()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? Guid.Parse(GetUser()?.GetUserId() ?? Guid.Empty.ToString()) : Guid.Empty;
            }

            public string ObterUserEmail()
            {
                return EstaAutenticado() ? GetUser()?.GetUserEmail() ?? "" : "";
            }

            public string ObterUserToken()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? GetUser()?.GetUserToken() ?? "" : "";
            }

            public string ObterUserRefreshToken()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? GetUser()?.GetUserRefreshToken() ?? "" : "";
            }

            // *** MÉTODO CRÍTICO CORRIGIDO ***
            public bool EstaAutenticado()
            {
                // Verifica se HttpContext existe E se a identidade existe E se está autenticada
                return GetUser()?.Identity?.IsAuthenticated ?? false; // Retorna false se qualquer parte for nula
            }

            public bool PossuiRole(string role)
            {
                // Verifica se HttpContext existe antes de chamar IsInRole
                return GetUser()?.IsInRole(role) ?? false; // Retorna false se HttpContext ou User for nulo
            }

            public IEnumerable<Claim> ObterClaims()
            {
                // Retorna claims ou uma coleção vazia se HttpContext ou User for nulo
                return GetUser()?.Claims ?? Enumerable.Empty<Claim>();
            }

            public HttpContext ObterHttpContext()
            {
                // Retorna o HttpContext diretamente (pode ser nulo, o chamador deve saber)
                return _accessor?.HttpContext;
            }

            // Os métodos abaixo já usavam o operador null-conditional (?.), o que é bom,
            // mas vamos garantir consistência usando GetUser() onde apropriado.
            public string ObterNomeCompleto()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetNomeCompleto() ?? string.Empty;
            }

            public string ObterApelido()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetApelido() ?? string.Empty;
            }

            public DateTime? ObterDataNascimento()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetDataNascimento(); // Já retorna nullable
            }

            public string ObterImgProfilePath()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetImgProfilePath() ?? string.Empty;
            }
        }
    }
}
