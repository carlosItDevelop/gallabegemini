namespace VelzonModerna.Workers
{
    public class PagamentoRecebidoWorker : BackgroundService
    {
        private readonly ILogger<PagamentoRecebidoWorker> _logger;

        public PagamentoRecebidoWorker(ILogger<PagamentoRecebidoWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan interval = TimeSpan.FromMinutes(6);
            using PeriodicTimer timer = new PeriodicTimer(interval);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"Pagamento Recebido em: {DateTime.UtcNow.TimeOfDay}, de: PAGADOR");
            }
        }
    }
}
