using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class Token : AuditModel<int>
    {
        [Required]
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string Value { get; set; }

        public int UsedCount { get; set; }

        public User Owner { get; set; }

        public int OwnerId { get; set; }
    }
}
