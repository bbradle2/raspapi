namespace raspapi.Interfaces
{
    public interface IGpioObjectsWaitEventHandler
    {
        bool Set();
        bool WaitOne(int millSeconds);
    }
}