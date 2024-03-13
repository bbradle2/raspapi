using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiGpioController
    {
        void StartGpio(WebApplication app);
      
    }
}