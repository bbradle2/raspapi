namespace first_test;

//using System.Data.Common;
using System.Device.Gpio;
//using MySqlConnector;
//using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Controllers;

class Program
{

    //private static readonly GpioController controller = new GpioController();
    
    // private static void SendMessageToTerminal(string message) 
    // {
    //     Console.WriteLine(message);
    // }

    //  private static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
    //  {
    //     gpController.Write(pin, PinValue.Low);
    //     gpController.ClosePin(pin);
    //      SendMessageToTerminal("SIGINT Received...");
    //  }

    // private static void OnSigTerm(object? sender, EventArgs e)
    // {
    //     gpController.Write(pin, PinValue.Low);
    //     gpController.ClosePin(pin);
    //     SendMessageToTerminal("SIGTERM Received...");
    // }

    static void Main(string[] args)
    {

        // AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
        // Console.CancelKeyPress += OnSigInt;

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<GpioController>();
        builder.Services.AddSingleton<IRaspberryPiController, RaspberryPiController>();

        var app = builder.Build();
        var piController = app.Services.GetService<IRaspberryPiController>();
        piController!.Start(app);

        app.Use(async (context, next) =>
        {
            try
            {
                // Do work that can write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            } 
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        });
      
        app.Run();

    }
}
