using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiGpioController
    {
        public void StartGpio(WebApplication app);
      
    }
}