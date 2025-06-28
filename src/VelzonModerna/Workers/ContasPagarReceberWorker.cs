namespace VelzonModerna.Workers
{
    public class ContasPagarReceberWorker : BackgroundService
    {
        private readonly ILogger<ContasPagarReceberWorker> _logger;

        public ContasPagarReceberWorker(ILogger<ContasPagarReceberWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan interval = TimeSpan.FromMinutes(5);
            using PeriodicTimer timer = new PeriodicTimer(interval);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"Pagamento pendente para: {DateTime.UtcNow.TimeOfDay}");
            }
        }
    }
}
