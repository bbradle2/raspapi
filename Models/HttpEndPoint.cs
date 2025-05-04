using System.Text.Json.Serialization;

namespace raspapi.Models
{
    public class HttpEndPoint 
    {
        public string? HttpMethod { get; set; }
        public string? HttpCallEndPoint { get; set; }
    }

}