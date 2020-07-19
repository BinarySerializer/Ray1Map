using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// RayLev data for the Mapper
    /// </summary>
    public class Mapper_RayLev : R1TextSerializable
    {
        /// <summary>
        /// The DES CMD manifest
        /// </summary>
        public Dictionary<string, string> DESManifest { get; set; }

        public override void Read(R1TextParser parser) {
            // Create the dictionary
            DESManifest = new Dictionary<string, string>();

            string firstValue;
            while ((firstValue = parser.ReadValue()) != null)
                // Add the item
                DESManifest.Add(firstValue, parser.ReadValue());
        }
    }
}