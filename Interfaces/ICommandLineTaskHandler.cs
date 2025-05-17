using raspapi.Models;
using System.Device.Gpio;

namespace raspapi.Interfaces
{
    public interface ICommandLineTaskHandler
    {
        void Handle(WebApplication webApplication);
    }
}
