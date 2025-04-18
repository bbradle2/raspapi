using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace raspapi.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task DownloadDataAsync(this ControllerBase controller, Stream dataContent, string mimeType, string fileName, bool inlineFile, int httpStatusCode, params (string key, StringValues value)[] additionalHeaders)
        {
            SetFileDownloadHeaders(controller, mimeType, fileName, inlineFile, httpStatusCode, additionalHeaders);
            await DownloadDataAsync(controller, dataContent);
        }

        public static async Task DownloadDataAsync(this ControllerBase controller, Func<Stream, Task> writeResponseDataAction, string mimeType, string fileName, bool inlineFile, int httpStatusCode, params (string key, StringValues value)[] additionalHeaders)
        {
            SetFileDownloadHeaders(controller, mimeType, fileName, inlineFile, httpStatusCode, additionalHeaders);
            await DownloadDataAsync(controller, writeResponseDataAction);
        }

        public static async Task DownloadDataAsync(this ControllerBase controller, Func<Stream, Task> writeResponseDataAction)
        {
            await writeResponseDataAction(controller.Response.Body);
        }

        public static async Task DownloadDataAsync(this ControllerBase controller, Stream dataContent)
        {
            await dataContent.CopyToAsync(controller.Response.Body);
        }

        private static void SetFileDownloadHeaders(ControllerBase controller, string mimeType, string fileName, bool inlineFile, int statusCode, (string key, StringValues value)[] additionalHeaders)
        {
            controller.Response.ContentType = string.IsNullOrEmpty(mimeType) ? "application/octet-stream" : mimeType;

            if (!string.IsNullOrEmpty(fileName))
            {
                var disposition = new ContentDisposition()
                {
                    Inline = inlineFile,
                };

                if (!inlineFile)
                {
                    // Filename is only allowed when file is 'attachment'.
                    // URL encode the file name if any non-extended ASCII (> 255) characters are detected
                    var encodedFileName = fileName.Any(c => c > 255) ?
                        WebUtility.UrlEncode(fileName) :
                        fileName;
                    disposition.FileName = encodedFileName;
                }

                controller.Response.Headers.Append(HeaderNames.ContentDisposition, disposition.ToString());
            }

            if (statusCode > 0)
            {
                controller.Response.StatusCode = statusCode;
            }

            if (additionalHeaders != null && additionalHeaders.Length > 0)
            {
                foreach (var (key, value) in additionalHeaders)
                {
                    controller.Response.Headers.Append(key, value);
                }
            }
        }
    }
}