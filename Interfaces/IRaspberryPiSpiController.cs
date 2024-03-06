using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiSpiController
    {
        public void StartSpi(WebApplication app);
      
    }
}