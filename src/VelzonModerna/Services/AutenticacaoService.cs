// Em VelzonModerna/Services/AutenticacaoService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using VelzonModerna.Configuration.Extensions;

namespace VelzonModerna.Services
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly IAspNetUser _user;
        private readonly IAuthenticationService _authenticationService;
        private readonly string _apiBaseUrl; // Campo para armazenar a URL base

        public AutenticacaoService(HttpClient httpClient,
                                   IOptions<AppSettingsMvc> settings,
                                   IAspNetUser user,
                                   IAuthenticationService authenticationService)
        {
            _apiBaseUrl = settings.Value.AutenticacaoUrl.TrimEnd('/'); // Armazena a URL base, removendo a barra final se houver
            httpClient.BaseAddress = new Uri(_apiBaseUrl + "/"); // Adiciona a barra para o BaseAddress do HttpClient

            _httpClient = httpClient;
            _user = user;
            _authenticationService = authenticationService;
        }

        // ... outros métodos do AutenticacaoService ...

        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterConteudo(usuarioLogin);
            // As rotas relativas aqui (ex: "/api/identidade/logar") serão combinadas com o BaseAddress
            var response = await _httpClient.PostAsync("/api/identidade/logar", loginContent); // Rota relativa ao BaseAddress

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = ObterConteudo(usuarioRegistro);
            var response = await _httpClient.PostAsync("/api/identidade/autenticar", registroContent); // Rota relativa

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }

        public async Task<UsuarioRespostaLogin> UtilizarRefreshToken(string refreshToken)
        {
            var refreshTokenContent = ObterConteudo(refreshToken);
            var response = await _httpClient.PostAsync("/api/identidade/refresh-token", refreshTokenContent); // Rota relativa

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }


        public async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            var token = ObterTokenFormatado(resposta.AccessToken);
            var claims = new List<Claim>();

            claims.Add(new Claim("JWT", resposta.AccessToken));
            claims.Add(new Claim("RefreshToken", resposta.RefreshToken.ToString()));

            foreach (var claimDoToken in token.Claims)
            {
                if (claimDoToken.Type == "role")
                {
                    claims.Add(new Claim(ClaimTypes.Role, claimDoToken.Value, claimDoToken.ValueType, claimDoToken.Issuer, claimDoToken.OriginalIssuer));
                } else
                {
                    claims.Add(claimDoToken);
                }
            }
            var claimsIdentity = new ClaimsIdentity(claims,
                                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                                    ClaimTypes.Name,
                                                    ClaimTypes.Role);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true
            };

            await _authenticationService.SignInAsync(
                _user.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task Logout()
        {
            await _authenticationService.SignOutAsync(
                _user.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                null);
        }

        public static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }

        public bool TokenExpirado()
        {
            var jwt = _user.ObterUserToken();
            if (jwt is null)
                return false;

            var token = ObterTokenFormatado(jwt);
            return token.ValidTo.ToLocalTime() < DateTime.Now;
        }

        public async Task<bool> RefreshTokenValido()
        {
            var resposta = await UtilizarRefreshToken(_user.ObterUserRefreshToken());

            if (resposta.AccessToken != null && resposta.ResponseResult == null)
            {
                await RealizarLogin(resposta);
                return true;
            }
            return false;
        }

        // IMPLEMENTAÇÃO DO NOVO MÉTODO
        public string GetApiBaseUrl()
        {
            return _apiBaseUrl; // Retorna a URL base armazenada (ex: "https://localhost:5013")
        }
    }
}