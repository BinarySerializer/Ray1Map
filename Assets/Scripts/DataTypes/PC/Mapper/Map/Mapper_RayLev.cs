using System;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// RayLev data for the Mapper
    /// </summary>
    public class Mapper_RayLev : MapperEngineSerializable
    {
        /// <summary>
        /// The DES CMD manifest
        /// </summary>
        public Dictionary<string, string> DESManifest { get; set; }

        public override void Read(MapperEngineCommandParser parser) {
            // Create the dictionary
            DESManifest = new Dictionary<string, string>();

            string firstValue;
            while ((firstValue = parser.NextValue()) != null)
                // Add the item
                DESManifest.Add(firstValue, parser.NextValue());
        }
    }
}