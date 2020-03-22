using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Event : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; }
        
        public Pointer UnkPointer2 { get; set; }
        
        public uint UnkPointer3 { get; set; }
        
        /// <summary>
        /// Pointer to the event commands
        /// </summary>
        public Pointer UnkPointer4 { get; set; }
        
        public Pointer CommandsPointer { get; set; }
        
        public uint UnkPointer5 { get; set; }

        public uint Unknown1 { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public ushort YPosition { get; set; }

        public byte[] Unknown2 { get; set; }

        public ushort Unknown3 { get; set; }

        public ushort Unknown4 { get; set; }

        // Always 254?
        public ushort Unknown5 { get; set; }

        public byte[] Unknown6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unknown7 { get; set; }

        public ushort Etat { get; set; }

        public ushort SubEtat { get; set; }

        public ushort Unknown8 { get; set; }

        public ushort Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort Hitpoints { get; set; }
        
        public byte UnkGroup { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public byte Type { get; set; }

        // NOTE: Maybe a byte?
        public ushort HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: "UnkPointer1");
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: "UnkPointer2");
            UnkPointer3 = s.Serialize(UnkPointer3, name: "UnkPointer3");
            UnkPointer4 = s.SerializePointer(UnkPointer4, name: "UnkPointer4");
            CommandsPointer = s.SerializePointer(CommandsPointer, name: "CommandsPointer");
            UnkPointer5 = s.Serialize(UnkPointer5, name: "UnkPointer5");
            Unknown1 = s.Serialize(Unknown1, name: "Unknown1");

            // This appears to always be 0
            if (Unknown1 != 0)
                Debug.Log($"PS1 event unk1 is {Unknown1}");

            XPosition = s.Serialize(XPosition, name: "XPosition");
            YPosition = s.Serialize(YPosition, name: "YPosition");

            Unknown2 = s.SerializeArray<byte>(Unknown2, 16, name: "Unknown2");
            Unknown3 = s.Serialize(Unknown3, name: "Unknown3");
            Unknown4 = s.Serialize(Unknown4, name: "Unknown4");
            Unknown5 = s.Serialize(Unknown5, name: "Unknown5");
            Unknown6 = s.SerializeArray<byte>(Unknown6, 28, name: "Unknown6");

            OffsetBX = s.Serialize(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize(OffsetBY, name: "OffsetBY");
            
            Unknown7 = s.Serialize(Unknown7, name: "Unknown7");

            Etat = s.Serialize(Etat, name: "Etat");
            SubEtat = s.Serialize(SubEtat, name: "SubEtat");

            Unknown8 = s.Serialize(Unknown8, name: "Unknown8");
            Unknown9 = s.Serialize(Unknown9, name: "Unknown9");

            OffsetHY = s.Serialize(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize(FollowSprite, name: "FollowSprite");

            Hitpoints = s.Serialize(Hitpoints, name: "Hitpoints");

            UnkGroup = s.Serialize(UnkGroup, name: "UnkGroup");

            Type = s.Serialize(Type, name: "Type");

            HitSprite = s.Serialize(HitSprite, name: "HitSprite");

            Unknown10 = s.SerializeArray<byte>(Unknown10, 10, name: "Unknown10");
        }
    }
}