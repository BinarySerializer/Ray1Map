using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_EventUnk2 : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; } // Unknown 2 times Unknown1 dwords
        
        public Pointer UnkPointer2 { get; set; } // Struct: Unknown2 dwords, then 2 ushorts.

        public ushort Unknown1 { get; set; }

        public ushort Unknown2 { get; set; }
        
        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize pointers
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: "UnkPointer1");
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: "UnkPointer2");
            Unknown1 = s.Serialize<ushort>(Unknown1, name: "Unknown1");
            Unknown2 = s.Serialize<ushort>(Unknown2, name: "Unknown2");
        }
    }
}