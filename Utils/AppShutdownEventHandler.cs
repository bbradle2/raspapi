namespace raspapi.Utils
{
    public class AppShutdownWaitEventHandler
    {
        private readonly EventWaitHandle _eventHandle;

        public AppShutdownWaitEventHandler()
        {
            _eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        public bool WaitOne(int millSeconds)
        {
            return _eventHandle.WaitOne(millSeconds);
            

        }
        public bool Set()
        {
            return _eventHandle.Set();
        }
        



       

    }
}