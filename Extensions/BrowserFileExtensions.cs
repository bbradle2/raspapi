using Microsoft.AspNetCore.Components.Forms;

namespace raspapi.Extensions
{
    public static class BrowserFileExtensions
    {
        public static async Task<byte[]> GetBytesAsync(this IBrowserFile file, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[file.Size];
            await file.OpenReadStream(cancellationToken: cancellationToken).ReadExactlyAsync(buffer, cancellationToken);
            return buffer;
        }

        public static byte[] GetBytes(this IBrowserFile file)
        {
            var buffer = new byte[file.Size];
            file.OpenReadStream().ReadExactly(buffer);
            return buffer;
        }

        // public static string GetReadableFileSize(this IBrowserFile file, bool fullName = false, IStringLocalizer? localizer = null)
        // {
        //     return FileHelper.GetReadableFileSize(file.Size, fullName, localizer != null ? s => localizer[s] : null);
        // }
    }

}