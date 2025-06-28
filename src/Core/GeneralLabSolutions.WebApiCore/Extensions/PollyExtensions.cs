using Polly;

using Polly.Retry; // Para AsyncRetryPolicy
using Polly.CircuitBreaker; // Para AsyncCircuitBreakerPolicy
using System;
using System.Net.Http; // Para HttpResponseMessage
using Microsoft.Extensions.Logging;
using Polly.Extensions.Http; // Para logging

namespace GeneralLabSolutions.WebApiCore.Extensions
{
    public static class PollyHttpPolicies
    {
        /// <summary>
        /// Define uma política de retentativa com backoff exponencial.
        /// </summary>
        /// <param name="retryCount">O número máximo de tentativas.</param>
        /// <param name="logger">Opcional: Logger para registrar as tentativas.</param>
        /// <returns>Uma política de retentativa assíncrona.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy(int retryCount = 3, ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Lida com erros comuns de rede (5XX, 408) ou HttpRequestException
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Exemplo: Retentar em NotFound também
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Backoff exponencial: 2s, 4s, 8s...
                    onRetry: (outcome, timespan, attempt, context) =>
                    {
                        var msg = $"Tentando novamente (tentativa {attempt} de {retryCount}) para {context.PolicyKey} em {context.OperationKey}, " +
                                  $"após {timespan.TotalSeconds:n1}s, devido a: {outcome.Result?.StatusCode}.";
                        if (logger != null)
                        {
                            logger.LogWarning(outcome.Exception, msg);
                        } else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(msg);
                            Console.ResetColor();
                        }
                    }
                );
        }

        /// <summary>
        /// Define uma política de Circuit Breaker.
        /// </summary>
        /// <param name="eventsAllowedBeforeBreaking">Número de eventos consecutivos antes de abrir o circuito.</param>
        /// <param name="durationOfBreakInSeconds">Duração em segundos que o circuito permanecerá aberto.</param>
        /// <param name="logger">Opcional: Logger para registrar os estados do circuito.</param>
        /// <returns>Uma política de Circuit Breaker assíncrona.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(
            int eventsAllowedBeforeBreaking = 5,
            int durationOfBreakInSeconds = 30,
            ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => (int)msg.StatusCode >= 500) // Considerar erros 5xx para o circuit breaker
                .CircuitBreakerAsync(
                    eventsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(durationOfBreakInSeconds),
                    onBreak: (result, timespan, context) =>
                    {
                        var msg = $"Circuito aberto por {timespan.TotalSeconds:n1}s para {context.PolicyKey} em {context.OperationKey} " +
                                  $"devido a: {result.Result?.StatusCode}.";
                        if (logger != null)
                        {
                            logger.LogError(result.Exception, msg);
                        } else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(msg);
                            Console.ResetColor();
                        }
                    },
                    onReset: (context) =>
                    {
                        var msg = $"Circuito resetado (fechado) para {context.PolicyKey} em {context.OperationKey}.";
                        if (logger != null)
                        {
                            logger.LogInformation(msg);
                        } else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(msg);
                            Console.ResetColor();
                        }
                    },
                    onHalfOpen: () =>
                    {
                        var msg = $"Circuito em estado de semi-aberto. Próxima chamada tentará fechar.";
                        if (logger != null)
                        {
                            logger.LogWarning(msg);
                        } else
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(msg);
                            Console.ResetColor();
                        }
                    }
                );
        }

        /// <summary>
        /// Política de Timeout simples.
        /// </summary>
        /// <param name="timeoutInSeconds">Tempo máximo de espera em segundos.</param>
        /// <returns>Uma política de timeout.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GetHttpTimeoutPolicy(int timeoutInSeconds = 30)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeoutInSeconds));
        }
    }
}