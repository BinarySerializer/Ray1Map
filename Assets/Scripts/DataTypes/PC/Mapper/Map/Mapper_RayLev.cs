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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Read)
            {
                // Create the dictionary
                DESManifest = new Dictionary<string, string>();

                // Use a command parser
                using (var parser = new MapperEngineCommandParser(serializer.BaseStream))
                {
                    string firstValue;

                    while ((firstValue = parser.NextValue()) != null)
                        // Add the item
                        DESManifest.Add(firstValue, parser.NextValue());
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}