using System.Text.Json.Serialization;

namespace Daktbot.Common.Entities
{
    /// <summary>
    /// Base entity from which all storage entities are derived.
    /// </summary>
    public abstract record BaseEntity
    {

        public string Id { get; init; }

        /// <summary>
        /// CosmosDb _etag.  Used for CosmosDB optimistic concurrency.
        /// </summary>
        [JsonPropertyName("_etag")]
        public string? Etag { get; init; }

        /// <summary>
        /// The time this entity was created.
        /// </summary>
        public DateTime CreationDate { get; init; } = DateTime.MinValue;

        /// <summary>
        /// The time this entity was last updated.
        /// </summary>
        public DateTime UpdatedDate { get; init; } = DateTime.Now;

        /// <summary>
        /// The time that this entity must be completely deleted.
        /// </summary>
        public DateTime RetentionCleanupDate { get; init; } = DateTime.Now.AddYears(7);

        /// <summary>
        /// Used to tombstone unused objects.
        /// </summary>
        public bool IsDeleted { get; init; } = false;

        /// <summary>
        /// Additional information related to the retrieval request.  Currently used to return seed information
        /// for fake generated data.
        /// </summary>
        public string? RequestInformation { get; init; }

    }
}