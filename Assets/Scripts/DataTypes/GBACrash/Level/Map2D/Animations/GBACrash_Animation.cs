namespace R1Engine
{
    public class GBACrash_Animation : R1Serializable
    {
        public Pointer FrameTablePointer { get; set; }

        // Hitbox and anim frame?
        public AnimRect HitBox { get; set; }
        public AnimRect RenderBox { get; set; }

        public byte PaletteIndex { get; set; }
        public byte AnimSpeed { get; set; } // Speed + 1
        public byte FramesCount { get; set; }
        public byte Byte_13 { get; set; }

        // Serialized from pointers

        public ushort[] FrameTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FrameTablePointer = s.SerializePointer(FrameTablePointer, name: nameof(FrameTablePointer));
            HitBox = s.SerializeObject<AnimRect>(HitBox, name: nameof(HitBox));
            RenderBox = s.SerializeObject<AnimRect>(RenderBox, name: nameof(RenderBox));
            PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
            AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
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