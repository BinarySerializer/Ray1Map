namespace R1Engine
{
    // For the flags, buttons etc.
    public class GBAIsometric_Sprite : R1Serializable
    {
        public int Width { get; set; } // In 8x8 tiles
        public int Height { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public bool Is8Bit { get; set; }
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer LookupBufferPositionsPointer { get; set; }
        public Pointer NamePointer { get; set; }


        //Parsed
        public uint CanvasWidth => Util.NextPowerOfTwo((uint)Width);
        public uint CanvasHeight => Util.NextPowerOfTwo((uint)Height);
        public ushort[] LookupBufferPositions { get; set; }
        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<byte>((bitFunc) => {
                Width = bitFunc(Width, 4, name: nameof(Width));
                Height = bitFunc(Height, 4, name: nameof(Height));
            });
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            LookupBufferPositionsPointer = s.SerializePointer(LookupBufferPositionsPointer, name: nameof(LookupBufferPositionsPointer));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            LookupBufferPositions = s.DoAt(LookupBufferPositionsPointer, () => s.SerializeArray<ushort>(LookupBufferPositions, CanvasWidth * CanvasHeight, name: nameof(LookupBufferPositions)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}