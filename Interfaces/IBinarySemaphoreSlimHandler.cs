namespace raspapi.Interfaces;

public interface IBinarySemaphoreSlimHandler
{
    int Release(); 
    Task WaitAsync();
}
