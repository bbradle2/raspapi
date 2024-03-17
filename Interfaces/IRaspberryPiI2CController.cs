using Microsoft.AspNetCore.Builder;

namespace raspapi.Interfaces
{
    public interface IRaspberryPiI2CController
    {
        void StartI2C(WebApplication app);
      
    }
}