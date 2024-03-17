using Microsoft.AspNetCore.Builder;

namespace raspapi.Interfaces
{
    public interface IRaspberryPiSpiController
    {
        void StartSpi(WebApplication app);
      
    }
}