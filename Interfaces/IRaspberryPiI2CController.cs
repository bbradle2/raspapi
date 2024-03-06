using Microsoft.AspNetCore.Builder;

namespace first_test.Interfaces
{
    public interface IRaspberryPiI2CController
    {
        public void StartI2C(WebApplication app);
      
    }
}