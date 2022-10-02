using CeskyRozhlasMiner.Lib.Common;
using System;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class Song : AuditModel<int>
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public DateTime PlayedAt { get; set; }
        public RozhlasStation SourceStation { get; set; }
    }
}
