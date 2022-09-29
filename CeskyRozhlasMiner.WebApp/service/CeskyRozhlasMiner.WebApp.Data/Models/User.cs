using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class User : AuditModel<int>
    {
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string DisplayName { get; set; }

        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string PasswordHash { get; set; }

        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string Email { get; set; }
    }
}
