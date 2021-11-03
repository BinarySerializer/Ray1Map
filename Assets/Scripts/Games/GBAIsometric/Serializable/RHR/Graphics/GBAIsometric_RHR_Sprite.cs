using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    // For the flags, buttons etc.
    public class GBAIsometric_RHR_Sprite : BinarySerializable
    {
        public GBAIsometric_RHR_SpriteInfo Info { get; set; }
        public bool Is8Bit { get; set; }
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer LookupBufferPositionsPointer { get; set; }
        public Pointer NamePointer { get; set; }

        //Parsed
        public ushort[] LookupBufferPositions { get; set; }
        public string Name { get; set; }
        public byte[] Sprite { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Info = s.SerializeObject<GBAIsometric_RHR_SpriteInfo>(Info, name: nameof(Info));
            Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            LookupBufferPositionsPointer = s.SerializePointer(LookupBufferPositionsPointer, name: nameof(LookupBufferPositionsPointer));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            LookupBufferPositions = s.DoAt(LookupBufferPositionsPointer, () => s.SerializeArray<ushort>(LookupBufferPositions, Info.CanvasWidth * Info.CanvasHeight, name: nameof(LookupBufferPositions)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            s.DoEncoded(new RHR_SpriteEncoder(Is8Bit, Info, LookupBufferPositions, GraphicsDataPointer.Value), () => {
                Sprite = s.SerializeArray<byte>(Sprite, s.CurrentLength, name: nameof(Sprite));
            });
            //s.DoEncoded(new RHR_SpriteEncoder(Is8Bit, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
            //    byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));
            //    //Util.ByteArrayToFile(Context.BasePath + $"sprites/Full_{Offset.StringAbsoluteOffset}.bin", fullSheet);
            //});
        }
    }
}