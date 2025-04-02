namespace raspapi.Utils
{
    public class BinarySemaphoreSlim
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int IntialCount = 1;
        private readonly int CurrentCount = 1;

        public BinarySemaphoreSlim()
        {
            _semaphore = new SemaphoreSlim(IntialCount, CurrentCount);
        }
        public async Task WaitAsync()
        {
            await _semaphore.WaitAsync();
        }

        public int Release()
        {
            return _semaphore.Release();
        }

    }
}