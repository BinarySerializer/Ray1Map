using System;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// RayLev data for the Mapper
    /// </summary>
    public class Mapper_RayLev : IBinarySerializable
    {
        /// <summary>
        /// The DES CMD manifest
        /// </summary>
        public Dictionary<string, string> DESManifest { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Create the dictionary
            DESManifest = new Dictionary<string, string>();

            // Use a command parser
            using (var parser = new MapperEngineCommandParser(deserializer.BaseStream))
            {
                string firstValue;

                while ((firstValue = parser.NextValue()) != null)
                    // Add the item
                    DESManifest.Add(firstValue, parser.NextValue());
            }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}