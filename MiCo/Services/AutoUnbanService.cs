namespace MiCo.Services
{
    public class AutoUnbanService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public AutoUnbanService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15)); // Call every 15 minutes
            return Task.CompletedTask;
        }

        public void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var justiceService = scope.ServiceProvider.GetRequiredService<IJusticeService>();
                justiceService.Unban(); // Call Unban method from JusticeService
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Change(TimeSpan.Zero, TimeSpan.FromMinutes(15)); // Reset timer
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
