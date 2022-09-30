using CeskyRozhlasMiner.Lib.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    [Owned]
    public class PlaylistSourceStation : AuditModel<int>
    {
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string Description { get; set; }

        [Required]
        public RozhlasStation Station { get; set; }
    }
}
