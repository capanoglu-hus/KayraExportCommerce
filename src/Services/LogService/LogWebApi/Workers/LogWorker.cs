namespace LogService.Workers
{
    /*redisten gelecek log ve eventler için kullanılacak*/
    public class LogWorker : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Başlangıç işlemleri
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Durdurma işlemleri
            return Task.CompletedTask;
        }
    }
}
