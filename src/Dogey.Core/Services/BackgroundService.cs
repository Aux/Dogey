using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _task = ExecuteAsync(_cancellationTokenSource.Token);
            
            if (_task.IsCompleted)
                return _task;
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_task == null)
                return;

            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
