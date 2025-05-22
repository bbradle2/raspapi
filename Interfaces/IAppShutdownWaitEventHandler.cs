namespace raspapi.Interfaces
{
    public interface IAppShutdownWaitEventHandler
    {
        bool Set();
        bool WaitOne(int millSeconds);
    }
}
