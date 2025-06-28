// No projeto MVC: VelzonModerna/Services/UserAdminMvcService.cs
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System; // Para ArgumentNullException e InvalidOperationException
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VelzonModerna.Configuration.Extensions; // Para AppSettingsMvc
// using VelzonModerna.Helpers; // Removido se IdentityRoleApiHelper está aqui dentro
using VelzonModerna.ViewModels;

namespace VelzonModerna.Services
{
    public class UserAdminMvcService : Service, IUserAdminMvcService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiAdminUrl; // URL para /api/admin/
        private readonly string _baseApiRoleUrl;  // URL para /api/role/

        // CONSTRUTOR CORRIGIDO: Sem 'string baseApiRoleUrl' como parâmetro
        public UserAdminMvcService(HttpClient httpClient, IOptions<AppSettingsMvc> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (settings?.Value == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (string.IsNullOrEmpty(settings.Value.UserAdminUrl))
            {
                throw new InvalidOperationException($"{nameof(AppSettingsMvc.UserAdminUrl)} não pode ser nula ou vazia.");
            }
            if (string.IsNullOrEmpty(settings.Value.AutenticacaoUrl))
            {
                throw new InvalidOperationException($"{nameof(AppSettingsMvc.AutenticacaoUrl)} não pode ser nula ou vazia.");
            }

            // A URL base para endpoints de ADMIN de usuário.
            // O HttpClient injetado já deve ter esta BaseAddress configurada pelo Program.cs
            _baseApiAdminUrl = settings.Value.UserAdminUrl.TrimEnd('/') + "/";
            // Se o httpClient.BaseAddress não for igual a _baseApiAdminUrl, pode haver um problema de configuração no Program.cs
            // Para este serviço, o BaseAddress do _httpClient DEVE ser _baseApiAdminUrl.

            // A URL base para endpoints de ROLE (que estão em /api/role/)
            var autenticacaoBaseUrl = settings.Value.AutenticacaoUrl.TrimEnd('/');
            _baseApiRoleUrl = autenticacaoBaseUrl + "/api/role/";
        }

        // Implementação do método da interface que você adicionou
        public string GetUserAdminApiBaseUrl() => _baseApiAdminUrl;

        // Seus métodos existentes...
        // ... (AdicionarUsuarioRoleAsync, AtivarDesativarUsuarioAsync, etc.)

        public async Task<ResponseResult> AdicionarUsuarioRoleAsync(AdicionarUsuarioRoleDto dto)
        {
            // Esta chamada usa a BaseAddress do _httpClient, que é _baseApiAdminUrl
            var response = await _httpClient.PostAsJsonAsync("adicionar-usuario-role", dto);

            if (!TratarErrosResponse(response))
            {
                return await DeserializarObjetoResponse<ResponseResult>(response);
            }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Operação realizada com sucesso." };
        }

        // ... outros métodos CRUD de usuário que usam rotas relativas ao _baseApiAdminUrl ...
        public async Task<ResponseResult> AtivarDesativarUsuarioAsync(string userId, bool ativar)
        {
            var response = await _httpClient.PutAsync($"usuarios/{userId}/ativar-inativar?ativar={ativar}", null);
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Operação realizada com sucesso." };
        }
        public async Task<ResponseResult> AtualizarSenhaAdminAsync(AtualizarSenhaDto dto)
        {
            _erros.Clear();
            AdicionarErro("Endpoint para administrador alterar senha de usuário não implementado na API ou serviço MVC.");

            return new ResponseResult
            {
                Status = 501,
                Title = "Não Implementado",
                Errors = new ResponseErrorMessages { Mensagens = new List<string>(_erros) }
            };
        }
        public async Task<ResponseResult> AtualizarUsuarioAsync(string userId, AtualizarUsuarioDto dto)
        {
            if (userId != dto.UserId)
            {
                _erros.Clear(); // Limpa erros anteriores desta instância do serviço para esta chamada
                AdicionarErro("Inconsistência no ID do usuário para atualização."); // Adiciona à lista _erros

                return new ResponseResult
                {
                    Status = 400,
                    Title = "Requisição Inválida",
                    // Popula o ResponseResult.Errors com as mensagens da lista _erros
                    Errors = new ResponseErrorMessages { Mensagens = new List<string>(_erros) }
                };
            }
            var response = await _httpClient.PutAsJsonAsync($"usuarios/{userId}", dto);
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Usuário atualizado com sucesso." };
        }
        public async Task<ResponseResult> BloquearDesbloquearUsuarioAsync(string userId, int? minutosBloqueio = null)
        {
            string url = $"usuarios/{userId}/bloquear-desbloquear";
            if (minutosBloqueio.HasValue)
            { url += $"?minutosBloqueio={minutosBloqueio.Value}"; }
            var response = await _httpClient.PutAsync(url, null);
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Operação realizada com sucesso." };
        }
        public async Task<ResponseResult> CriarUsuarioAsync(CriarUsuarioDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("criar-usuario", dto);
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            var responseData = await DeserializarObjetoResponse<ResponseResult>(response);
            if (responseData?.Errors?.Mensagens?.Any() == true)
            { return responseData; }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Usuário criado com sucesso." };
        }
        public async Task<ResponseResult> ExcluirClaimDoUsuarioAsync(string userId, string claimType, string claimValue)
        {
            var response = await _httpClient.DeleteAsync($"usuarios/{userId}/claims?claimType={Uri.EscapeDataString(claimType)}&claimValue={Uri.EscapeDataString(claimValue)}");
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Claim excluída com sucesso." };
        }
        public async Task<ResponseResult> ExcluirUsuarioAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"usuarios/{userId}");
            if (!TratarErrosResponse(response))
            { return await DeserializarObjetoResponse<ResponseResult>(response); }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Usuário excluído com sucesso." };
        }
        public async Task<IEnumerable<UsuarioClaim>> ObterClaimsDoUsuarioAsync(string userId, string? tipo = null, string? valor = null)
        {
            string url = $"usuarios/{userId}/claims";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(tipo))
                queryParams.Add($"tipo={Uri.EscapeDataString(tipo)}");
            if (!string.IsNullOrEmpty(valor))
                queryParams.Add($"valor={Uri.EscapeDataString(valor)}");
            if (queryParams.Any())
            { url += "?" + string.Join("&", queryParams); }
            var response = await _httpClient.GetAsync(url);
            if (!TratarErrosResponse(response))
            { return Enumerable.Empty<UsuarioClaim>(); }
            var claimsApi = await DeserializarObjetoResponse<List<ClaimResponseApiHelper>>(response);
            return claimsApi?.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value }) ?? Enumerable.Empty<UsuarioClaim>();
        }
        public async Task<IEnumerable<UsuarioClaim>> ObterTodasClaimsSistemaAsync(string? tipo = null, string? valor = null)
        {
            string url = "claims";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(tipo))
                queryParams.Add($"tipo={Uri.EscapeDataString(tipo)}");
            if (!string.IsNullOrEmpty(valor))
                queryParams.Add($"valor={Uri.EscapeDataString(valor)}");
            if (queryParams.Any())
            { url += "?" + string.Join("&", queryParams); }
            var response = await _httpClient.GetAsync(url);
            if (!TratarErrosResponse(response))
            { return Enumerable.Empty<UsuarioClaim>(); }
            var claimsApi = await DeserializarObjetoResponse<List<ClaimResponseApiHelper>>(response);
            return claimsApi?.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value }) ?? Enumerable.Empty<UsuarioClaim>();
        }
        public async Task<UserDto> ObterUsuarioPorIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"usuarios/{userId}");
            if (!TratarErrosResponse(response))
            {
                // var errorResponse = await DeserializarObjetoResponse<ResponseResult>(response); // Para debug
                return null;
            }
            return await DeserializarObjetoResponse<UserDto>(response);
        }
        public async Task<IEnumerable<UserResponseDto>> ObterTodosUsuariosAsync()
        {
            var response = await _httpClient.GetAsync("usuarios");
            if (!TratarErrosResponse(response))
            { return null; }
            return await DeserializarObjetoResponse<IEnumerable<UserResponseDto>>(response);
        }
        public async Task<IEnumerable<UserDto>> ObterUsuariosPorClaimAsync(string tipo, string valor)
        {
            var response = await _httpClient.GetAsync($"claims/usuarios?tipo={Uri.EscapeDataString(tipo)}&valor={Uri.EscapeDataString(valor)}");
            if (!TratarErrosResponse(response))
            { return Enumerable.Empty<UserDto>(); }
            return await DeserializarObjetoResponse<IEnumerable<UserDto>>(response);
        }
        public async Task<(ResponseResult Response, string ImagemPath)> UploadImagemPerfilAsync(string userId, IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
            {
                return (new ResponseResult { Status = 400, Title = "Erro de Validação", Errors = new ResponseErrorMessages { Mensagens = new List<string> { "Nenhuma imagem fornecida." } } }, null);
            }
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(userId), "UserId");
            using var imageStream = imagem.OpenReadStream();
            var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagem.ContentType);
            content.Add(imageContent, "Imagem", imagem.FileName);
            var response = await _httpClient.PostAsync("usuarios/imagem", content);
            if (!TratarErrosResponse(response))
            {
                var errorResult = await DeserializarObjetoResponse<ResponseResult>(response);
                return (errorResult, null);
            }
            var imagemPath = await response.Content.ReadAsStringAsync();
            return (new ResponseResult { Status = (int)response.StatusCode, Title = "Upload realizado com sucesso." }, imagemPath);
        }


        // Lista para armazenar erros de validação ou de processo antes de montar o ResponseResult
        private readonly List<string> _erros = new List<string>();
        protected void AdicionarErro(string erro) // Este AdicionarErro é local.
        {
            _erros.Add(erro);
        }
        // Se você tem um método AdicionarErroProcessamentoView no seu Service base, use-o.
        // Por exemplo, para ser usado como this.AdicionarErroProcessamentoView(mensagem);
        // protected abstract void AdicionarErroProcessamentoView(string erro); // Se for abstrato no Service base
        // Ou, se for concreto no Service base:
        // protected void AdicionarErroProcessamentoView(string mensagem) { /* implementação no Service base */ }


        // --- MÉTODOS PARA ROLES ---
        public async Task<IEnumerable<RoleViewModel>> ObterTodasRolesAsync()
        {
            var requestUrl = _baseApiRoleUrl + "roles"; // Usa a URL completa para /api/role/
            var response = await _httpClient.GetAsync(requestUrl);

            if (!TratarErrosResponse(response))
            {
                return Enumerable.Empty<RoleViewModel>();
            }
            var identityRoles = await response.Content.ReadFromJsonAsync<List<IdentityRoleApiHelper>>();
            return identityRoles?.Select(r => new RoleViewModel { Id = r.Id, Nome = r.Name }) ?? Enumerable.Empty<RoleViewModel>();
        }

        public async Task<IList<string>> ObterRolesDoUsuarioAsync(string userId)
        {
            var requestUrl = $"{_baseApiRoleUrl}usuarios/{userId}/roles"; // Usa a URL completa para /api/role/
            var response = await _httpClient.GetAsync(requestUrl);

            if (!TratarErrosResponse(response))
            {
                return new List<string>();
            }
            return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
        }

        public async Task<ResponseResult> RemoverUsuarioDeRoleAsync(string userId, string roleName)
        {
            // Esta chamada usa a BaseAddress do _httpClient, que é _baseApiAdminUrl
            // e a rota relativa "usuarios/{userId}/roles/{roleName}" que assumimos estar no UserAdminController da API
            var response = await _httpClient.DeleteAsync($"usuarios/{userId}/roles/{Uri.EscapeDataString(roleName)}");

            if (!TratarErrosResponse(response))
            {
                return await DeserializarObjetoResponse<ResponseResult>(response);
            }
            return new ResponseResult { Status = (int)response.StatusCode, Title = "Role removida do usuário com sucesso." };
        }

        // Classe auxiliar interna para desserializar IdentityRole
        private class IdentityRoleApiHelper
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string NormalizedName { get; set; }
            public string ConcurrencyStamp { get; set; }
            // Adicione outras propriedades se necessário
        }
        // Classe auxiliar interna para desserializar Claim
        private class ClaimResponseApiHelper
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public string Issuer { get; set; }
            public string OriginalIssuer { get; set; }
            // Adicione outras propriedades se a API as serializar e você precisar delas
        }
    }
}