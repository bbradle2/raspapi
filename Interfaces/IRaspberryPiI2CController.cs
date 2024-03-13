using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiI2CController
    {
        void StartI2C(WebApplication app);
      
    }
}