using System.Net.Http.Headers;
using System.Text;
using GeneralLabSolutions.Domain.Services;
using GeneralLabSolutions.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VelzonModerna.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _http;
        private readonly OpenAISettings _cfg;
        private readonly ILogger<OpenAIService> _log;

        public OpenAIService(HttpClient http,
                             IOptions<OpenAISettings> cfg,
                             ILogger<OpenAIService> log)
        {
            _http = http;
            _cfg = cfg.Value;
            _log = log;

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _cfg.ApiKey);
        }

        // O erro ocorre porque valores padrão de parâmetros em C# devem ser constantes em tempo de compilação.
        // No seu método, você está tentando usar valores de instância (_cfg.Model, _cfg.MaxTokens, _cfg.Temperature) como valores padrão, o que não é permitido.
        // Para corrigir, remova os valores padrão da assinatura e atribua-os dentro do método caso os argumentos sejam nulos ou não definidos.

        public async Task<string> ChatAsync(
            IEnumerable<ChatMessage> messages,
            string? model = null,
            int? maxTokens = null,
            double? temperature = null)
        {
            model ??= _cfg.Model;
            maxTokens ??= _cfg.MaxTokens;
            temperature ??= _cfg.Temperature;

            var payload = new
            {
                model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }),
                max_tokens = maxTokens,
                temperature
            };

            var json = JsonConvert.SerializeObject(payload);
            _log.LogDebug("OpenAI Payload: {json}", json);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(_cfg.BaseUrl, content);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _log.LogError("OpenAI status {code}: {body}", resp.StatusCode, body);
                var msg = SafeError(body) ?? $"Erro OpenAI ({resp.StatusCode})";
                return msg;
            }

            return ExtractAnswer(body) ?? "(Resposta vazia)";
        }

        /* helpers privados */
        private static string? ExtractAnswer(string json)
        {
            var j = JObject.Parse(json);
            return j ["choices"]? [0]? ["message"]? ["content"]?.ToString();
        }
        private static string? SafeError(string json)
        {
            try
            {
                return JObject.Parse(json) ["error"]? ["message"]?.ToString();
            } catch { return null; }
        }
    }
}
