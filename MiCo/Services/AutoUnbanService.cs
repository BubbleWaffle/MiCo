using MiCo.Data;

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15)); //Call every 15 minutes
            return Task.CompletedTask;
        }

        public void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var banService = scope.ServiceProvider.GetRequiredService<UnbanService>();
                banService.Unban(); //Call Unban method from UnbanService
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Change(TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
