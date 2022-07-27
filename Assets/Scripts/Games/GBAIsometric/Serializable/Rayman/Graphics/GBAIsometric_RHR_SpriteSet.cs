using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_SpriteSet : BinarySerializable
    {
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer LookupBufferPositionsPointer { get; set; }
        public Pointer SpriteInfosPointer { get; set; }
        public bool Is8Bit { get; set; }
        public byte SpriteCount { get; set; }
        public Pointer NamePointer { get; set; }

        public ushort[][] LookupBufferPositions { get; set; }
        public GBAIsometric_RHR_SpriteInfo[] SpriteInfos { get; set; }
        public string Name { get; set; }
        public byte[][] Sprites { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, name: nameof(GraphicsDataPointer))?.ResolveObject(s);
            LookupBufferPositionsPointer = s.SerializePointer(LookupBufferPositionsPointer, name: nameof(LookupBufferPositionsPointer));
            SpriteInfosPointer = s.SerializePointer(SpriteInfosPointer, name: nameof(SpriteInfosPointer));
            Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
            SpriteCount = s.Serialize<byte>(SpriteCount, name: nameof(SpriteCount));
            s.Serialize<ushort>(default, name: "Padding");
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            SpriteInfos = s.DoAt(SpriteInfosPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_SpriteInfo>(SpriteInfos, SpriteCount, name: nameof(SpriteInfos)));
            s.DoAt(LookupBufferPositionsPointer, () =>
            {
                if (LookupBufferPositions == null)
                    LookupBufferPositions = new ushort[SpriteCount][];

                for (int i = 0; i < LookupBufferPositions.Length; i++)
                {
                    LookupBufferPositions[i] = s.SerializeArray<ushort>(LookupBufferPositions[i], SpriteInfos[i].CanvasWidth * SpriteInfos[i].CanvasHeight, name: $"{nameof(LookupBufferPositions)}[{i}]");
                } });
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            if (Sprites == null)
                Sprites = new byte[SpriteCount][];

            for (int i = 0; i < Sprites.Length; i++)
                s.DoEncoded(new RHR_SpriteEncoder(Is8Bit, SpriteInfos[i], LookupBufferPositions[i], GraphicsDataPointer.Value), () => {
                    Sprites[i] = s.SerializeArray<byte>(Sprites[i], s.CurrentLength, name: $"{nameof(Sprites)}[{i}]");
                });
        }
    }
}