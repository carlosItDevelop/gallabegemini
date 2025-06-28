using System.Net;
using System.Net.Http.Headers;
using GeneralLabSolutions.WebApiCore.Usuario; // Para IAspNetUser
using Microsoft.Extensions.DependencyInjection; // Para IServiceProvider e CreateScope
using System.IO; // Para MemoryStream
using System.Collections.Generic; // Para KeyValuePair
using System.Threading.Tasks; // Para Task
using System.Threading; // Para CancellationToken
using System.Net.Http; // Para Http* classes

namespace VelzonModerna.Services
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IAspNetUser _user;
        private readonly IServiceProvider _serviceProvider;

        private static bool _isRefreshingToken = false;
        private static readonly object _lock = new object();

        public HttpClientAuthorizationDelegatingHandler(IAspNetUser user, IServiceProvider serviceProvider)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _user.ObterUserToken();
            if (!string.IsNullOrEmpty(token))
            {
                // Limpar qualquer Authorization header existente antes de adicionar o novo
                request.Headers.Authorization = null;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool canAttemptRefresh = false;
                // Gate simples para evitar múltiplas chamadas de refresh simultâneas
                lock (_lock)
                {
                    if (!_isRefreshingToken)
                    {
                        _isRefreshingToken = true;
                        canAttemptRefresh = true;
                    }
                }

                if (canAttemptRefresh)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var autenticacaoService = scope.ServiceProvider.GetRequiredService<IAutenticacaoService>();

                            // A API retornou 401. Verificar se o token no contexto do MVC realmente expirou.
                            if (autenticacaoService.TokenExpirado())
                            {
                                var refreshTokenFoiValidadoComSucesso = await autenticacaoService.RefreshTokenValido();

                                if (refreshTokenFoiValidadoComSucesso)
                                {
                                    // Token renovado, e o HttpContext/claims foram atualizados pelo RealizarLogin.
                                    var novoToken = _user.ObterUserToken(); // Pega o novo token
                                    if (!string.IsNullOrEmpty(novoToken))
                                    {
                                        // Clonar a requisição original
                                        var newRequest = await CloneHttpRequestMessageAsync(request);
                                        // Aplicar o novo token
                                        newRequest.Headers.Authorization = null; // Limpa o antigo
                                        newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", novoToken);

                                        response.Dispose(); // Descarta a resposta 401 original
                                        // Reenviar a requisição com o novo token
                                        response = await base.SendAsync(newRequest, cancellationToken);
                                    }
                                }
                                // Se refreshTokenFoiValidadoComSucesso for false, a resposta 401 original prosseguirá
                                // e será tratada pelo ExceptionMiddleware (redirecionando para login).
                            }
                        }
                    } catch (Exception ex) // Capturar exceções durante o processo de refresh
                    {
                        // Logar a exceção (ex.LogError(ex, "Erro ao tentar renovar o token de acesso.");)
                        // Deixar a resposta 401 original prosseguir.
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            _isRefreshingToken = false; // Libera o gate
                        }
                    }
                } else // Outra thread já está tentando o refresh
                {
                    // Esperar um pouco para dar chance da outra thread completar
                    await Task.Delay(1500, cancellationToken).ConfigureAwait(false); // Aumentei um pouco o delay

                    var tokenAtualizadoPelaOutraThread = _user.ObterUserToken();
                    // Verifica se o token foi de fato atualizado e se é diferente do que esta requisição tentou usar
                    if (!string.IsNullOrEmpty(tokenAtualizadoPelaOutraThread) &&
                        (request.Headers.Authorization == null || request.Headers.Authorization.Parameter != tokenAtualizadoPelaOutraThread))
                    {
                        // Token parece ter sido renovado por outra chamada, tentar reenviar com o novo token
                        var newRequest = await CloneHttpRequestMessageAsync(request);
                        newRequest.Headers.Authorization = null;
                        newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAtualizadoPelaOutraThread);
                        response.Dispose();
                        response = await base.SendAsync(newRequest, cancellationToken);
                    }
                    // Se o token não mudou, a resposta 401 original prosseguirá.
                }
            }

            return response;
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Version = req.Version
            };

            // Copiar conteúdo
            if (req.Content != null)
            {
                var ms = new MemoryStream();
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);
                if (req.Content.Headers != null)
                {
                    foreach (var h in req.Content.Headers)
                    {
                        clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
                    }
                }
            }

            // Copiar Headers da Requisição (exceto Authorization, que será setado depois)
            foreach (var header in req.Headers.Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copiar Options
            foreach (var option in req.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
            }

            return clone;
        }
    }
}