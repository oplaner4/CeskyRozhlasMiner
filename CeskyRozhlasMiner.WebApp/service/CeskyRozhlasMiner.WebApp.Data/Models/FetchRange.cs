using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class FetchRange : AuditModel<int>
    {
        [Required]
        [Column(Order = 0)]
        public DateTime From { get; set; }

        [Required]
        [Column(Order = 1)]
        public DateTime To { get; set; }

        public virtual ICollection<FetchRangeSourceStation> SourceStations { get; set; } = new HashSet<FetchRangeSourceStation>();
    }
}
