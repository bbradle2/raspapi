using System.Diagnostics;

namespace raspapi.LinuxExtensions
{
    public static class LinuxExtensions
    {
        public static string ExecuteBashScript(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            string? result = null;

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                process.Dispose();
                process = null;
            }

            return result!;
        }
    }
}