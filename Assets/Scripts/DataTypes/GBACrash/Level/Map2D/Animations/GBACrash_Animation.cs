namespace R1Engine
{
    public class GBACrash_Animation : R1Serializable
    {
        public Pointer FrameTablePointer { get; set; }

        // Hitbox and anim frame?
        public AnimRect Rect_0 { get; set; }
        public AnimRect Rect_1 { get; set; }
        public byte PaletteIndex { get; set; }
        public byte Byte_11 { get; set; } // Speed + 1?
        public byte FramesCount { get; set; }
        public byte Byte_13 { get; set; }


        // Serialized from pointers

        public ushort[] FrameTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FrameTablePointer = s.SerializePointer(FrameTablePointer, name: nameof(FrameTablePointer));
            Rect_0 = s.SerializeObject<AnimRect>(Rect_0, name: nameof(Rect_0));
            Rect_1 = s.SerializeObject<AnimRect>(Rect_1, name: nameof(Rect_1));
            PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
            Byte_11 = s.Serialize<byte>(Byte_11, name: nameof(Byte_11));
            FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
            Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));
            s.SerializeArray<byte>(new byte[4], 4, name: "Padding");

            FrameTable = s.DoAt(FrameTablePointer, () => s.SerializeArray<ushort>(FrameTable, FramesCount, name: nameof(FrameTable)));
        }

        public class AnimRect : R1Serializable
        {
            public short X { get; set; }
            public short Y { get; set; }
            public short Width { get; set; }
            public short Height { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                X = s.Serialize<short>(X, name: nameof(X));
                Y = s.Serialize<short>(Y, name: nameof(Y));
                Width = s.Serialize<short>(Width, name: nameof(Width));
                Height = s.Serialize<short>(Height, name: nameof(Height));
            }
        }
    }
}