using BinarySerializer;
using System;

namespace R1Engine.Jade
{
    public class ANI_Shape : Jade_File
    {
        public byte LastCanal { get; set; } // Max 64
        public ANI_Shape_Canal[] Canals { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LastCanal = s.Serialize<byte>(LastCanal, name: nameof(LastCanal));
            Canals = s.SerializeObjectArray(Canals, Math.Min((byte)64, LastCanal), name: nameof(Canals));
        }

        public class ANI_Shape_Canal : BinarySerializable
        {
            public sbyte Canal { get; set; }
            public byte AI_Canal_01 { get; set; }
            public byte AI_Canal_02 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Canal = s.Serialize<sbyte>(Canal, name: nameof(Canal));

                if (Canal >= 0 && Canal < 64)
                {
                    AI_Canal_01 = s.Serialize<byte>(AI_Canal_01, name: nameof(AI_Canal_01));
                    AI_Canal_02 = s.Serialize<byte>(AI_Canal_02, name: nameof(AI_Canal_02));
                }
            }
        }
    }
}