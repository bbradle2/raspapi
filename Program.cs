namespace first_test;

using System.Data.Common;
using System.Device.Gpio;
using MySqlConnector;
using Dapper;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;



public static class LinuxExtensions
{
    public static string Execute(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        string ?result = null;

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        try
        {
            process.Start();
            result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        } 
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            process.Dispose();
            process = null;
        }

        return result!;
    }
}
class Program
{
    
    private static readonly GpioController controller = new GpioController();
    //private static bool ledOn = true;

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
        
        
        int milliseconds = 2000;
        try
        {
            var command = "cat /proc/cpuinfo";
            Console.WriteLine(command.Execute());

            SendMessageToTerminal($"Opening LED Pin {pin}.");
            controller.OpenPin(pin, PinMode.Output);
            SendMessageToTerminal($"Turn On LED {pin}.");
            controller.Write(pin, PinValue.High);

            SendMessageToTerminal($"Sleeping For {milliseconds} Milli Seconds.");
            Thread.Sleep(2000);

            SendMessageToTerminal($"Turn Off LED {pin}.");
            controller.Write(pin, PinValue.Low);
            
        }  
        catch (Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        } 
        finally 
        {
            controller.ClosePin(pin);
        }


    }

}
