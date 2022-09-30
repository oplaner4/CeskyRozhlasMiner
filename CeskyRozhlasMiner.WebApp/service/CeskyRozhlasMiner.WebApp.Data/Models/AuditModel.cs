using System;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public abstract class AuditModel<TType> : BaseModel<TType>
    {
        /// <summary>
        /// Tracks when this entity was first persisted to the database.
        /// </summary>
        /// <remarks>Managed at the Entity Framework-level. Do not manually set.</remarks>
        public DateTime CreatedDate { get; internal set; }

        /// <summary>
        /// Tracks when this entity was last updated.
        /// </summary>
        /// <remarks>Managed at the Entity Framework-level. Do not manually set.</remarks>
        public DateTime UpdatedDate { get; internal set; }

        /// <summary>
        /// True indicates that this entity has been already deleted and should not be used 
        /// in command handlers.
        /// </summary>
        public bool Deleted { get; set; }
    }
}
