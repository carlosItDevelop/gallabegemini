using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GeneralLabSolutions.WebApiCore.Usuario
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtém o User ID (sub ou NameIdentifier) da Claim.
        /// Retorna string.Empty se a claim não for encontrada.
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                // Considerar lançar ArgumentNullException se principal não deveria ser nulo aqui.
                // Mas para um método de extensão, retornar um valor padrão pode ser aceitável.
                return string.Empty;
            }

            // Tenta primeiro "sub" (padrão OIDC para ID do usuário)
            var claim = principal.FindFirst("sub") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o email do usuário da Claim "email" ou ClaimTypes.Email.
        /// Retorna string.Empty se a claim não for encontrada.
        /// </summary>
        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // Tenta primeiro "email" (comum em JWTs) e depois o ClaimTypes.Email padrão
            var claim = principal.FindFirst("email") ?? principal.FindFirst(ClaimTypes.Email);
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o Access Token (JWT) que foi armazenado como uma claim.
        /// Retorna string.Empty se a claim "JWT" não for encontrada.
        /// </summary>
        public static string GetUserToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // A claim "JWT" foi adicionada manualmente em AutenticacaoService.RealizarLogin
            var claim = principal.FindFirst("JWT");
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o Refresh Token que foi armazenado como uma claim.
        /// Retorna string.Empty se a claim "RefreshToken" não for encontrada.
        /// </summary>
        public static string GetUserRefreshToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // A claim "RefreshToken" foi adicionada manualmente em AutenticacaoService.RealizarLogin
            var claim = principal.FindFirst("RefreshToken");
            return claim?.Value ?? string.Empty;
        }


        // Novos métodos para Claims adicionais
        public static string GetNomeCompleto(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("nome_completo") ?? principal.FindFirst(JwtRegisteredClaimNames.Name);
            return claim?.Value ?? string.Empty;
        }

        public static string GetApelido(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("apelido");
            return claim?.Value ?? string.Empty;
        }

        public static DateTime? GetDataNascimento(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var claim = principal.FindFirst("birthdate") ?? principal.FindFirst(ClaimTypes.DateOfBirth);
            if (claim?.Value != null)
            {
                // Se armazenou como "o" (round-trip date/time pattern)
                if (DateTime.TryParse(claim.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dataNascimento))
                {
                    return dataNascimento;
                }
                // Se armazenou como "yyyy-MM-dd"
                if (DateTime.TryParseExact(claim.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                {
                    return dataNascimento;
                }
            }
            return null;
        }

        public static string GetImgProfilePath(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("img_profile_path");
            return claim?.Value ?? string.Empty;
        }


    }
}