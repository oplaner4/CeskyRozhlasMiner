using System.Text.Json.Serialization;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class GoogleSignInDataDto
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("credential")]
        public string Credential { get; set; }

        [JsonPropertyName("select_by")]
        public string Select_by { get; set; }
    }
}
