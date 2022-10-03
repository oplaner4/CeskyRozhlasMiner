using CeskyRozhlasMiner.Lib.Common;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class FetchRangeSourceStation : AuditModel<int>
    {
        [Required]
        public RozhlasStation Station { get; set; }
    }
}
