using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace raspapi.Intercepts
{
    public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger, IConfiguration config)
    {

        private readonly ILogger<ApiIntercept> _logger = logger;
        private readonly IConfiguration _config = config;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Remove("GitSemVer");
            context.Response.Headers.Append("GitSemVer", GitVersionInformation.SemVer);
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
            {
                
                _logger.LogInformation("Authorized User {Environment.UserName}", Environment.UserName);
                // context.Request.EnableBuffering();
                // Stream originalBody = context.Response.Body;

                // try
                // {
                //     using var memStream = new MemoryStream();
                //     context.Response.Body = memStream;

                //     memStream.Position = 0;
                //     string responseBody = new StreamReader(memStream).ReadToEnd();
                //     var key = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
                //     var iv = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0D };

                //     memStream.Position = 0;
                //     byte[] data = EncryptStringToBytes_Aes(responseBody, key, iv);//Encrypt responseBody here
                //     memStream.Write(data, 0, data.Length);

                //     memStream.Position = 0;

                //     await memStream.CopyToAsync(originalBody);
                // }
                // finally
                // {
                //     context.Response.Body = originalBody;
                // }

                await next(context);
                return;
            }

            context.Response.StatusCode = 401;
            string unauthorizedUser = context.Request.Headers["AUTHORIZED_USER"]!;
            _logger.LogError("Unauthorized User {unauthorizedUser}", unauthorizedUser);
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;

        }

        public static async Task<byte[]> ReadToEndAsync(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(readBuffer.AsMemory(totalBytesRead, readBuffer.Length - totalBytesRead))) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                           
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        static async Task<byte[]> EncryptStringToBytes_AesAsync(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using System.Security.Cryptography.Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            // Create the streams used for encryption.
            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using StreamWriter swEncrypt = new(csEncrypt);
            await swEncrypt.WriteAsync(plainText);
            encrypted = msEncrypt.ToArray();

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
