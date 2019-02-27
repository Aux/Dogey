using System;

namespace Dogey.Models
{
    public class ModelLog
    {
        /// <summary> This log's id </summary>
        public ulong Id { get; set; }
        /// <summary> The logged model's id </summary>
        public ulong ModelId { get; set; }
        /// <summary> The model's type name </summary>
        public string Type { get; set; }
        /// <summary> The date and time this model was created </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary> The date and time this model was last updated </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary> The date and time this model was last deleted </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary> The date and time this model will be removed from the database after being deleted </summary>
        public DateTime? ExpiresAt { get; set; }
    }
}
