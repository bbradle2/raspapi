namespace raspapi.Interfaces
{
    public interface IBinarySemaphoreSlim
    {
        int Release();
        Task WaitAsync();
    }
}