using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Cosmos
{
    /// <summary>
    /// This class contains the default parameters which are always available on each and every record in the cosmos db database.
    /// </summary>
    public class Document : Serialization.Extendable
    {
        /// <summary>
        /// Contains a string which will change if the content of the document changes.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("_etag")]
        public virtual string ETag { get; set; }
        /// <summary>
        /// Contains the ID of the document.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public virtual string ID { get; set; }
        /// <summary>
        /// Contains the partition key of the document.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("pk")]
        public virtual string PartitionKey { get; set; }
        /// <summary>
        /// Indicates when the record was created/updated
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("_ts")]
        public virtual long TimeStamp { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("_self")]
        public string Self { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("_rid")]
        public string _rid { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("_attachments")]
        public string Attachments { get; set; }
    }

    /// <summary>
    /// Represents the root class of a 'parameter' document.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParameterDocument<T> : Document
    {
        /// <summary>
        /// The stored info in the document.
        /// </summary>
        public T Data { get; set; }
    }
}
