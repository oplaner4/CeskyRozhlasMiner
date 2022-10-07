using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class FetchRange : AuditModel<int>
    {
        [Column(Order = 0)]
        public DateTime From { get; set; }

        [Column(Order = 1)]
        public DateTime To { get; set; }

        public virtual ICollection<FetchRangeSourceStation> SourceStations { get; set; } = new HashSet<FetchRangeSourceStation>();
    }
}
