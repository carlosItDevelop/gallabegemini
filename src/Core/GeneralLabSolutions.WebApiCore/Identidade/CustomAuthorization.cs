using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GeneralLabSolutions.WebApiCore.Identidade
{
    public static class CustomAuthorization
    {
        public static bool EhSuperAdmin(HttpContext context)
        {
            return context.User.IsInRole("SuperAdmin");
        }

        public static bool ValidarClaimsUsuario(HttpContext context, string claimType, string claimValue)
        {
            return context.User.Identity != null && context.User.Identity.IsAuthenticated &&
                   context.User.Claims.Any(c => c.Type == claimType && c.Value.Contains(claimValue)); // Usar c.Value.Equals para correspondência exata se necessário
        }

        public static bool PossuiRole(HttpContext context, string role)
        {
            if (string.IsNullOrEmpty(role))
                return false;
            return context.User.IsInRole(role);
        }
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Autoriza o acesso com base em uma claim específica e, opcionalmente, uma role alternativa.
        /// SuperAdmins sempre têm acesso.
        /// </summary>
        /// <param name="claimType">O tipo da claim necessária.</param>
        /// <param name="claimValue">O valor da claim necessária.</param>
        /// <param name="roleAlternativa">Uma role que, se o usuário possuir, também concederá acesso caso a claim principal não seja atendida.</param>
        public ClaimsAuthorizeAttribute(string claimType, string claimValue, string? roleAlternativa = null)
            : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object [] { claimType, claimValue, roleAlternativa ?? string.Empty };
        }
    }

    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claimPrincipal;
        private readonly string? _roleAlternativaAtributo;

        public RequisitoClaimFilter(string claimType, string claimValue, string roleAlternativaRecebida)
        {
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentNullException(nameof(claimType));
            // claimValue pode ser nulo/vazio se sua lógica permitir (ex: HasClaim(type))
            // Para o seu caso atual, onde value é importante, pode-se manter a verificação ou não.
            // if (string.IsNullOrEmpty(claimValue)) throw new ArgumentNullException(nameof(claimValue));

            _claimPrincipal = new Claim(claimType, claimValue ?? string.Empty);
            _roleAlternativaAtributo = string.IsNullOrEmpty(roleAlternativaRecebida) ? null : roleAlternativaRecebida;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            if (CustomAuthorization.EhSuperAdmin(context.HttpContext))
            {
                return; // SuperAdmin sempre tem acesso
            }

            bool possuiClaimPrincipal = CustomAuthorization.ValidarClaimsUsuario(
                context.HttpContext, _claimPrincipal.Type, _claimPrincipal.Value);

            if (possuiClaimPrincipal)
            {
                return; // Acesso permitido pela claim principal
            }

            if (!string.IsNullOrEmpty(_roleAlternativaAtributo))
            {
                bool possuiRoleAlternativa = CustomAuthorization.PossuiRole(
                    context.HttpContext, _roleAlternativaAtributo);

                if (possuiRoleAlternativa)
                {
                    return; // Acesso permitido pela role alternativa
                }
            }

            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden); // Acesso negado
        }
    }
}