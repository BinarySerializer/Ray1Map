using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Image descriptor data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_ImageDescriptor : R1Serializable
    {
        public byte[] Unknown { get; set; }
        
        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            Unknown = s.SerializeArray<byte>(Unknown, 0x14, name: "Unknown");
        }
    }
}