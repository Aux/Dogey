namespace Dogey.Services
{
    /// <summary> Default implementation of a service </summary>
    public abstract class BackgroundService
    {


        public abstract void Start();
        public abstract void Stop();
    }
}
