namespace raspapi.Extensions
{
    
    using System.Diagnostics;
  

    public static class StringExtensions
    {
        
        public static async Task<string> ExecuteBashScriptAsync(this string cmd)
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
                result = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
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