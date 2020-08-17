using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Asm.Serialization
{
    /// <summary>
    /// Contains an object that may contain more properties then are made available via the class definition.
    /// When you deserialize an object to type "x" and it has fields which do not exist in that type those fields should remain available when serializing the item back to its original format.
    /// </summary>
    public class Extendable
    {
        /// <summary>
        /// This property will contain the extra information which is not (de)serialized to any other property.
        /// Technically we don't want to see in our intellisense since we should never need to access it.
        /// </summary>
        [JsonExtensionData, EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<string, JsonElement> _extra_props { get; set; }
    }
}
