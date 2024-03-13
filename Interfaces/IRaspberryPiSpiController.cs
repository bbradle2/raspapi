using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiSpiController
    {
        void StartSpi(WebApplication app);
      
    }
}