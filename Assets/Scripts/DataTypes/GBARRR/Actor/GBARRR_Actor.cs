namespace R1Engine
{
    public class GBARRR_Actor : R1Serializable
    {
        public short YPosition { get; set; }
        public short XPosition { get; set; }
        public byte[] Data1 { get; set; }
        public ushort Ushort_0A { get; set; }
        public ushort Ushort_0C { get; set; } // 2 bytes?
        public ushort Ushort_0E { get; set; }
        public ushort Ushort_10 { get; set; }
        public ushort RuntimeStateIndex { get; set; } // Is this correct? Is it a byte?
        public byte[] Data2 { get; set; }
        public uint RuntimeFunctionPointer { get; set; }
        public ushort Ushort_20 { get; set; }
        public short RuntimeXPosition { get; set; }
        public short RuntimeYPosition { get; set; }
        public byte RuntimeAnimIndex { get; set; } // Is this correct? Is it a byte?
        public byte Byte_27 { get; set; }
        public byte SpriteWidth { get; set; }
        public byte SpriteHeight { get; set; }
        public ushort Ushort_2A { get; set; } // Graphics index?
        public ushort Ushort_2C { get; set; }
        public ushort Ushort_2E { get; set; }
        public uint Uint_30 { get; set; }
        public uint Uint_34 { get; set; }
        public int RuntimeAnimFrame { get; set; } // Is this correct? Is it an int?
        public uint RuntimeAnimOffset { get; set; } // Is this correct? Changes with the anim index.

        public override void SerializeImpl(SerializerObject s)
        {
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            Data1 = s.SerializeArray<byte>(Data1, 6, name: nameof(Data1));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));
            Ushort_0C = s.Serialize<ushort>(Ushort_0C, name: nameof(Ushort_0C));
            Ushort_0E = s.Serialize<ushort>(Ushort_0E, name: nameof(Ushort_0E));
            Ushort_10 = s.Serialize<ushort>(Ushort_10, name: nameof(Ushort_10));
            RuntimeStateIndex = s.Serialize<ushort>(RuntimeStateIndex, name: nameof(RuntimeStateIndex));
            Data2 = s.SerializeArray<byte>(Data2, 8, name: nameof(Data2));
            RuntimeFunctionPointer = s.Serialize<uint>(RuntimeFunctionPointer, name: nameof(RuntimeFunctionPointer));
            Ushort_20 = s.Serialize<ushort>(Ushort_20, name: nameof(Ushort_20));
            RuntimeXPosition = s.Serialize<short>(RuntimeXPosition, name: nameof(RuntimeXPosition));
            RuntimeYPosition = s.Serialize<short>(RuntimeYPosition, name: nameof(RuntimeYPosition));
            RuntimeAnimIndex = s.Serialize<byte>(RuntimeAnimIndex, name: nameof(RuntimeAnimIndex));
            Byte_27 = s.Serialize<byte>(Byte_27, name: nameof(Byte_27));
            SpriteWidth = s.Serialize<byte>(SpriteWidth, name: nameof(SpriteWidth));
            SpriteHeight = s.Serialize<byte>(SpriteHeight, name: nameof(SpriteHeight));
            Ushort_2A = s.Serialize<ushort>(Ushort_2A, name: nameof(Ushort_2A));
            Ushort_2C = s.Serialize<ushort>(Ushort_2C, name: nameof(Ushort_2C));
            Ushort_2E = s.Serialize<ushort>(Ushort_2E, name: nameof(Ushort_2E));
            Uint_30 = s.Serialize<uint>(Uint_30, name: nameof(Uint_30));
            Uint_34 = s.Serialize<uint>(Uint_34, name: nameof(Uint_34));
            RuntimeAnimFrame = s.Serialize<int>(RuntimeAnimFrame, name: nameof(RuntimeAnimFrame));
            RuntimeAnimOffset = s.Serialize<uint>(RuntimeAnimOffset, name: nameof(RuntimeAnimOffset));
        }
    }
}