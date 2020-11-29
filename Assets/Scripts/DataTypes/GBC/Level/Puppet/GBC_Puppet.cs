namespace R1Engine
{
    public class GBC_Puppet : GBC_BaseBlock
    {
        public byte[] PuppetData { get; set; }

        // Parsed
        public GBC_TileKit TileKit { get; set; }
        public GBC_RomChannel[] Animations { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            PuppetData = s.SerializeArray<byte>(PuppetData, BlockSize, name: nameof(PuppetData));

            TileKit = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_TileKit>(TileKit, name: nameof(TileKit)));
            if(Animations == null) Animations = new GBC_RomChannel[OffsetTable.OffsetsCount - 1];
            for (int i = 0; i < Animations.Length; i++) {
                Animations[i] = s.DoAt(OffsetTable.GetPointer(i+1), () => s.SerializeObject<GBC_RomChannel>(Animations[i], name: $"{nameof(Animations)}[{i}]"));
            }
        }
    }
}