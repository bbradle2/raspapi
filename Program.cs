namespace first_test;

//using System.Data.Common;
using System.Device.Gpio;
//using MySqlConnector;
//using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using DataObjects;
using LinuxExtensions;
using System;

class Program
{

    //private static readonly GpioController controller = new GpioController();
    private static readonly int pin = 23;
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
        
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            // Do work that can write to the Response.
            await next.Invoke();
            // Do logging or other work that doesn't write to the Response.
        });
        //var config = app.Configuration;
        //var s = app.Services.GetService<GpioController>();

        app.MapGet("/cpuinfo", async (HttpContext context) =>
        {
            StringBuilder? retRes = new StringBuilder();
            
            try
            {
                var commandRes = await Task.Run(() => "cat /proc/cpuinfo".Execute());

                foreach (var c in commandRes)
                {
                    if (c == '\n' || c == '\t') continue;

                    retRes.Append(c);
                }

                CPUObject cpuObject = new CPUObject { Call = "CPUInfo", Content = retRes.ToString() };

                return Results.Ok(cpuObject);
            }
            finally
            {
                retRes = null;
            }
        });

        app.MapGet("/ledstatus", (GpioController gpioController) =>
        {
            ArgumentNullException.ThrowIfNull(gpioController);

            var openPin = gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                return Results.Json(openPin.Read().ToString());
            }
            finally
            {

            }
        });

        app.MapGet("/ledon", (GpioController gpioController) =>
        {
            ArgumentNullException.ThrowIfNull(gpioController);

            var openPin = gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                gpioController.Write(pin, PinValue.High);
                var pinValue = openPin.Read();
                return Results.Json(pinValue.ToString());
            }
            finally
            {

            }
        });

        app.MapGet("/ledoff", (HttpContext context, GpioController gpioController) =>
        {
            
            ArgumentNullException.ThrowIfNull(gpioController);

            var openPin = gpioController.OpenPin(pin, PinMode.Output);

            try
            {
                gpioController.Write(pin, PinValue.Low);
                var pinValue = openPin.Read();
                return Results.Json(pinValue.ToString());
            }
            finally
            {

            }
        });

        app.Run();

    }
}
