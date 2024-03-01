namespace first_test;

using System.Data.Common;
using System.Device.Gpio;
using MySqlConnector;
using Dapper;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using System.Text;
using DataObjects;
using LinuxExtensions;




class Program
{
    
    private static readonly GpioController controller = new GpioController();
    private static readonly int pin = 23;
    private static void SendMessageToTerminal(string message) 
    {
        Console.WriteLine(message);
    }
    private static void OnSigInt(object? sender, ConsoleCancelEventArgs e)
    {
        controller.Write(pin, PinValue.Low);
        controller.ClosePin(pin);
        SendMessageToTerminal("SIGINT Received...");
    }

    private static void OnSigTerm(object? sender, EventArgs e)
    {
        controller.Write(pin, PinValue.Low);
        controller.ClosePin(pin);
        SendMessageToTerminal("SIGTERM Received...");
    }

    static void Main(string[] args)
    {

        AppDomain.CurrentDomain.ProcessExit += OnSigTerm;
        Console.CancelKeyPress += OnSigInt;

        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/cpuinfo", async () =>
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

                return Results.Json(cpuObject);
            } 
            finally 
            {
                retRes = null;
            }
        });

        app.MapGet("/ledstatus", () =>
        {
            var openPin = controller.OpenPin(pin, PinMode.Output);

            try
            {        
                return Results.Json(openPin.Read().ToString());
            } 
            finally 
            {
                
            }
        });

        app.MapGet("/ledon", () =>
        {
            var openPin = controller.OpenPin(pin, PinMode.Output);

            try
            {
                controller.Write(pin, PinValue.High);
                var pinValue = openPin.Read();
                return Results.Json(pinValue.ToString());
            }
            finally
            {
                
            }
        });

        app.MapGet("/ledoff", () =>
        {
            var openPin = controller.OpenPin(pin, PinMode.Output);

            try
            {
                controller.Write(pin, PinValue.Low);
                var pinValue = openPin.Read();
                return Results.Json(pinValue.ToString());
            }
            finally
            {
                
            }
        });

        app.Run();
        controller.OpenPin(pin, PinMode.Output);
        controller.Write(pin, PinValue.Low);
        controller.ClosePin(pin);
        AppDomain.CurrentDomain.ProcessExit -= OnSigTerm;
        Console.CancelKeyPress -= OnSigInt;
    }
}
