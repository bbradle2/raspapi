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
    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");

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
        
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}
class Program
{
    
    private static readonly GpioController controller = new GpioController();
    private static bool ledOn = true;

    private static readonly int pin = 23;
    private static void SendMessageToTerminal(string message) 
    {
        Console.WriteLine("\r\n" + message);
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

        //Console.WriteLine("cat /proc/cpuinfo".Bash());
        Console.WriteLine(GitVersionInformation.SemVer);

        //SendMessageToTerminal("Blinking LED. Press Ctrl+C to end.");
        controller.OpenPin(pin, PinMode.Output);
        controller.Write(pin, PinValue.High);
        Thread.Sleep(3000);
        controller.Write(pin, PinValue.Low);
        controller.ClosePin(pin);
        //      Thread.Sleep(1000);

        // try
        // {
        //     controller.OpenPin(pin, PinMode.Output);
        // } 
        // catch (Exception ex) 
        // {
        //     Console.WriteLine(ex.ToString());
        //     return;
        // }

        //  while (true)
        //  {
        //      controller.Write(pin, (ledOn) ? PinValue.High : PinValue.Low);
        //      Thread.Sleep(1000);
        //      ledOn = !ledOn;
        //  }
       
    }

}
