using System;
using System.Threading.Tasks;

namespace Dogey.Services
{
    /// <summary> A service intended to run in the background </summary>
    public abstract class AsyncService : Service, IDisposable
    {
        protected Task _task;
        
        protected abstract Task StartTaskAsync();
        protected abstract Task StopTaskAsync();

        public void Dispose()
        {
            Stop();
        }
    }
}
