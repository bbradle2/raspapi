
using System.Device.Spi;
using System.Text;
using first_test.DataObjects;
using first_test.Interfaces;
using first_test.LinuxExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace first_test.Controllers
{
    public class RaspberryPiI2CController : IRaspberryPiI2CController
    {
        public void StartI2C(WebApplication app)  
        {

        }
    }

    
}