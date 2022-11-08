using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class User : AuditModel<int>
    {
        [Required]
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string Email { get; set; }

        public bool Verified { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
    }
}
