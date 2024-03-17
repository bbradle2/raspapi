using Microsoft.AspNetCore.Builder;

namespace raspapi.Interfaces
{
    public interface IRaspberryPiGpioController
    {
        void StartGpio(WebApplication app);
      
    }
}