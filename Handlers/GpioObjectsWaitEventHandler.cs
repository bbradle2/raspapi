using raspapi.Interfaces;

namespace raspapi.Handlers
{

    public class GpioObjectsWaitEventHandler : IGpioObjectsWaitEventHandler
    {
        private readonly EventWaitHandle _eventHandle;

        public GpioObjectsWaitEventHandler()
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