using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

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
